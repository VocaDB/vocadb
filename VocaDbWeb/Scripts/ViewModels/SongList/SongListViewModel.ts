
module vdb.viewModels.songList {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongListViewModel {

		constructor(
			urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository, private userRepo: rep.UserRepository, 
			private languageSelection: cls.globalization.ContentLanguagePreference,
			private listId: number,
			pvPlayerWrapperElement: HTMLElement) {

			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, 'pv-player', pvPlayerWrapperElement);
			this.playlistViewModel = new SongListPlayListViewModel(urlMapper, songListRepo, songRepo, userRepo, this.pvPlayerViewModel, languageSelection, listId);
			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);

			this.playlistMode.subscribe(mode => {
				if (mode)
					this.playlistViewModel.init();
			});

			this.updateResultsWithTotalCount();

		}

		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public playlistMode = ko.observable(false);
		public playlistViewModel: SongListPlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.songListRepo.getSongs(this.listId, null, pagingProperties,
				new cls.SongOptionalFields(cls.SongOptionalField.AdditionalNames, cls.SongOptionalField.ThumbUrl),
				this.languageSelection,
				(result) => {

					_.each(result.items, (item) => {

						var song = item.song;
						var songAny: any = song;

						if (song.pvServices && song.pvServices != 'Nothing') {
							songAny.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
							songAny.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
						} else {
							songAny.previewViewModel = null;
						}

					});

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(result.items);
					this.loading(false);

				});

		}

	}

}