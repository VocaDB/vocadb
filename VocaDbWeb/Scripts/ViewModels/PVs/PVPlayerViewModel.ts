
module vdb.viewModels.pvs {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;
	import serv = cls.pvs.PVService;

	export class PVPlayerViewModel {
		
		public static autoplayPVServicesString = "File, LocalFile, SoundCloud, Youtube";

		constructor(
			private urlMapper: UrlMapper,
			private songRepo: rep.SongRepository,
			userRepo: rep.UserRepository,
			pvPlayersFactory: PVPlayersFactory,
			autoplay?: boolean,
			shuffle?: boolean
			) {

			if (autoplay !== null && autoplay !== undefined)
				this.autoplay(autoplay);

			if (shuffle !== null && shuffle !== undefined)
				this.shuffle(shuffle);

			this.players = pvPlayersFactory.createPlayers(this.songFinishedPlayback);

			this.selectedSong.subscribe(song => {

				if (song == null) {

					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = null;
					}

					this.playerHtml("");
					this.ratingButtonsViewModel(null);
					return;					
				}

				userRepo.getSongRating(null, song.song.id, rating => {
					this.ratingButtonsViewModel(new PVRatingButtonsViewModel(userRepo, { id: song.song.id, vote: rating }, ui.showThankYouForRatingMessage));
				});

				// Use current player
				if (this.currentPlayer && this.songHasPVService(song, this.currentPlayer.service)) {

					this.loadPVId(this.currentPlayer.service, song.song.id, this.currentPlayer.play);

				} else { 

					// Detech old player
					if (this.currentPlayer) {
						this.currentPlayer.detach();
						this.currentPlayer = null;						
					}

					var services = this.autoplay() ? vdb.viewModels.pvs.PVPlayerViewModel.autoplayPVServicesString : null;

					// Load new player from server and attach it
					songRepo.pvPlayer(song.song.id, { elementId: pvPlayersFactory.playerElementId, enableScriptAccess: true, pvServices: services }, result => {

						this.playerHtml(result.playerHtml);
						this.playerService = serv[result.pvService];
						this.currentPlayer = this.players[result.pvService];

						if (this.currentPlayer) {
							this.currentPlayer.attach(false, () => {
								this.currentPlayer.play();
							});
						}

					});

				}

			});

			this.autoplay.subscribe(autoplay => {

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
					var newService = _.find(this.autoplayServices, s => this.songHasPVService(this.selectedSong(), s));
					if (newService) {

						this.playerService = newService;
						this.currentPlayer = this.players[serv[newService]];
						this.currentPlayer.attach(true, () => {
							this.loadPVId(this.currentPlayer.service, this.selectedSong().song.id, this.currentPlayer.play);
						});
						return;
						
					}

					// Case 3
					if (this.resetSong)
						this.resetSong();

				}

			});

		}

		public autoplay = ko.observable(false);
		private autoplayServices = [serv.File, serv.Youtube, serv.SoundCloud];
		private currentPlayer: IPVPlayer = null;

		private loadPVId = (service: serv, songId: number, callback: (pvId: string) => void) => {
			this.songRepo.getPvId(songId, service, callback);
		}

		private players: { [index: string]: IPVPlayer; };
		public nextSong: () => void;
		public playerHtml = ko.observable<string>(null);
		public playerService: serv = null;
		public ratingButtonsViewModel: KnockoutObservable<PVRatingButtonsViewModel> = ko.observable(null);
		public resetSong: () => void = null;
		public selectedSong = ko.observable<IPVPlayerSong>(null);
		private static serviceName = (service: serv) => serv[service];
		public shuffle = ko.observable(false);

		private songFinishedPlayback = () => {

			if (this.autoplay() && this.nextSong)
				this.nextSong();

		}

		private songHasPVService = (song: IPVPlayerSong, service: serv) => {
			return _.includes(song.song.pvServicesArray, service);
		}

		public songIsValid = (song: IPVPlayerSong) => {
			return !this.autoplay() || this.autoplayServices.some(s => _.includes(song.song.pvServicesArray, s));
		}

	}

	export interface IPVPlayerSong {

		song: dc.SongApiContract;

	}

	export interface IPVPlayer {

		// Attach the player by creating the JavaScript object, either to the currently playing element, or create a new element.
		// reset: whether to create a new player element
		// readyCallback: called when the player is ready
		attach: (reset?: boolean, readyCallback?: () => void) => void;

		detach: () => void;

		// Called when the currently playing song has finished playing. This will only be called if the player was attached.
		songFinishedCallback?: () => void;
		play: (pvId?: string) => void;
		service: cls.pvs.PVService;

	}

}