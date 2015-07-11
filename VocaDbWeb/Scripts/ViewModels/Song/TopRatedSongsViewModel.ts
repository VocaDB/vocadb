 
module vdb.viewModels.songs {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class TopRatedSongsViewModel {
		
		constructor(private urlMapper: vdb.UrlMapper, private songRepo: rep.SongRepository, private userRepo: rep.UserRepository, private languagePreference: number) {

			this.durationHours.subscribe(this.getSongs);
			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);
			
			this.getSongs();

		}

		public durationHours = ko.observable(168);

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		private getSongs = () => {
		
			$.getJSON(this.urlMapper.mapRelative('/api/songs/top-rated'), { durationHours: this.durationHours(), languagePreference: this.languagePreference },(songs: dc.SongApiContract[]) => {

				_.each(songs, (song: any) => {

					if (song.pvServices && song.pvServices != 'Nothing') {
						song.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
						song.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
					} else {
						song.previewViewModel = null;
					}

				});


				this.songs(songs);
			});
				
		}

		private pvServiceIcons: vdb.models.PVServiceIcons;

		public songs = ko.observableArray<dc.SongApiContract>(null);

	}

}