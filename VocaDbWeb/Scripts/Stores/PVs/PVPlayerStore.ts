import type { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { PVService } from '@/Models/PVs/PVService';
import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { PVRatingButtonsStore } from '@/Stores/PVRatingButtonsStore';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

export interface IPVPlayer {
	// Attach the player by creating the JavaScript object, either to the currently playing element, or create a new element.
	// reset: whether to create a new player element
	// readyCallback: called when the player is ready
	attach: (reset?: boolean, readyCallback?: () => void) => Promise<void>;

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

export class PVPlayerStore {
	static autoplayPVServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.SoundCloud,
		PVService.Youtube,
	];
	static autoplayPVServicesString = PVPlayerStore.autoplayPVServices.join(', ');

	@observable autoplay = false;
	private readonly autoplayServices = [
		PVService.File,
		PVService.Youtube,
		PVService.SoundCloud,
	];
	private readonly players: { [index: string]: IPVPlayer };
	nextSong?: () => void;
	@observable primaryPV?: PVContract;
	playerService?: PVService;
	@observable ratingButtonsStore?: PVRatingButtonsStore;
	resetSong?: () => void;
	@observable selectedSong?: IPVPlayerSong;
	@observable shuffle = false;

	constructor(
		values: GlobalValues,
		private readonly songRepo: SongRepository,
		userRepo: UserRepository,
		readonly pvPlayersFactory: PVPlayersFactory,
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
					this.primaryPV = undefined;
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
				if (false) {
				} else {
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
								this.primaryPV = {
									pvId: result.pvId,
									service: result.pvService,
								} as PVContract;
								this.playerService = result.pvService;
							});
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

					// Case 2
					const newService = this.autoplayServices.find((s) =>
						this.songHasPVService(this.selectedSong!, s),
					);
					if (newService) {
						this.playerService = newService;
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
		return song.song.pvServicesArray?.includes(service) ?? false;
	};

	songIsValid = (song: IPVPlayerSong): boolean => {
		return (
			!this.autoplay ||
			this.autoplayServices.some(
				(s) => song.song.pvServicesArray?.includes(s) ?? false,
			)
		);
	};
}
