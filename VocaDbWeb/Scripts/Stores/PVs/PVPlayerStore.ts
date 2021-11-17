import SongApiContract from '@DataContracts/Song/SongApiContract';
import PVService from '@Models/PVs/PVService';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import PVRatingButtonsStore from '@Stores/PVRatingButtonsStore';
import _ from 'lodash';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

import PVPlayersFactory from './PVPlayersFactory';

export interface IPVPlayer {
	// Attach the player by creating the JavaScript object, either to the currently playing element, or create a new element.
	// reset: whether to create a new player element
	// readyCallback: called when the player is ready
	attach: (reset?: boolean, readyCallback?: () => void) => void;

	detach: () => void;

	// Called when the currently playing song has finished playing. This will only be called if the player was attached.
	songFinishedCallback?: () => void;

	// Start playing the video.
	// pvId: ID of the PV being played. Note: this can be undefined, in which case the player should be attached and started playback without loading a new PV.
	play: (pvId?: string) => void;
	service: PVService;
}

export interface IPVPlayerSong {
	song: SongApiContract;
}

export default class PVPlayerStore {
	public static autoplayPVServicesString =
		'File, LocalFile, NicoNicoDouga, SoundCloud, Youtube';

	@observable public autoplay = false;
	private readonly autoplayServices = [
		PVService.File,
		PVService.Youtube,
		PVService.SoundCloud,
	];
	private currentPlayer?: IPVPlayer;
	private readonly players: { [index: string]: IPVPlayer };
	public nextSong?: () => void;
	@observable public playerHtml?: string;
	public playerService?: PVService;
	@observable public ratingButtonsStore?: PVRatingButtonsStore;
	public resetSong?: () => void;
	@observable public selectedSong?: IPVPlayerSong;
	@observable public shuffle = false;

	public constructor(
		values: GlobalValues,
		private readonly songRepo: SongRepository,
		userRepo: UserRepository,
		public readonly pvPlayersFactory: PVPlayersFactory,
		autoplay?: boolean,
		shuffle?: boolean,
	) {
		makeObservable(this);

		if (autoplay) this.autoplay = autoplay;
		if (shuffle) this.shuffle = shuffle;

		this.players = pvPlayersFactory.createPlayers(this.songFinishedPlayback);

		reaction(
			() => this.selectedSong,
			(song) => {
				if (!song) {
					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = undefined;
					}

					this.playerHtml = '';
					this.ratingButtonsStore = undefined;
					return;
				}

				userRepo
					.getSongRating({ userId: values.loggedUserId, songId: song.song.id })
					.then((rating) => {
						runInAction(() => {
							this.ratingButtonsStore = new PVRatingButtonsStore(userRepo, {
								id: song.song.id,
								vote: rating,
							});
						});
					});

				// Use current player
				if (
					this.currentPlayer &&
					this.songHasPVService(song, this.currentPlayer.service)
				) {
					this.loadPVId(this.currentPlayer.service, song.song.id).then((pvId) =>
						this.currentPlayer!.play(pvId),
					);
				} else {
					// Detech old player
					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = undefined;
					}

					const services = this.autoplay
						? PVPlayerStore.autoplayPVServicesString
						: undefined;

					// Load new player from server and attach it
					songRepo
						.pvPlayer({
							songId: song.song.id,
							params: {
								elementId: pvPlayersFactory.playerElementId,
								enableScriptAccess: true,
								pvServices: services,
							},
						})
						.then((result) => {
							runInAction(() => {
								this.playerHtml = result.playerHtml;
								this.playerService =
									PVService[result.pvService as keyof typeof PVService];
								this.currentPlayer = this.players[result.pvService];
							});

							if (this.currentPlayer) {
								this.currentPlayer.attach(false, () => {
									this.currentPlayer!.play();
								});
							}
						});
				}
			},
		);

		reaction(
			() => this.autoplay,
			(autoplay) => {
				if (autoplay) {
					/* 
						3 cases: 
						1) currently playing PV supports autoplay: no need to do anything (already attached)
						2) currently playing song has PV that supports autoplay with another player: switch player
						3) currently playing song doesn't have a PV that supports autoplay: switch song
					*/

					// Case 1
					if (this.currentPlayer) {
						return;
					}

					// Case 2
					const newService = _.find(this.autoplayServices, (s) =>
						this.songHasPVService(this.selectedSong!, s),
					);
					if (newService) {
						this.playerService = newService;
						this.currentPlayer = this.players[PVService[newService]];
						this.currentPlayer.attach(true, () => {
							this.loadPVId(
								this.currentPlayer!.service,
								this.selectedSong!.song.id,
							).then((pvId) => this.currentPlayer!.play(pvId));
						});
						return;
					}

					// Case 3
					this.resetSong?.();
				}
			},
		);
	}

	private loadPVId = (service: PVService, songId: number): Promise<string> =>
		this.songRepo.getPvId({ songId: songId, pvService: service });

	private songFinishedPlayback = (): void => {
		if (this.autoplay) this.nextSong?.();
	};

	private songHasPVService = (
		song: IPVPlayerSong,
		service: PVService,
	): boolean => {
		return _.includes(song.song.pvServicesArray, service);
	};

	public songIsValid = (song: IPVPlayerSong): boolean => {
		return (
			!this.autoplay ||
			this.autoplayServices.some((s) =>
				_.includes(song.song.pvServicesArray, s),
			)
		);
	};
}
