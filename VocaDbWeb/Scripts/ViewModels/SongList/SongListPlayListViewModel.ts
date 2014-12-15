/// <reference path="../../typings/youtube/youtube.d.ts" />

module vdb.viewModels.songList {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class SongListPlayListViewModel {

		constructor(
			private urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository,
			private userRepo: rep.UserRepository, 
			private pvPlayerViewModel: pvs.PVPlayerViewModel,
			private languageSelection: cls.globalization.ContentLanguagePreference, 
			private listId: number) {

			pvPlayerViewModel.nextSong = this.nextSong;
			pvPlayerViewModel.resetSong = () => {

				this.pvPlayerViewModel.selectedSong(_.find(this.page(), song => pvPlayerViewModel.songIsValid(song)));

			}

			pvPlayerViewModel.autoplay.subscribe(() => this.updateResultsWithTotalCount());

			this.hasMoreSongs = ko.computed(() => {
				return this.page().length < this.paging.totalItems();
			});

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

		}

		public formatLength = (length: number) => vdb.helpers.DateTimeHelper.formatFromSeconds(length);

		private getSongIndex = (song: viewModels.pvs.IPVPlayerSong) => {
			
			for (var i = 0; i < this.page().length; ++i) {
				if (this.page()[i].song.id == song.song.id)
					return i;
			}

			return -1;

		}

		private hasMoreSongs: KnockoutComputed<boolean>;

		public isInit = false;

		public init = () => {

			if (this.isInit)
				return;

			this.updateResultsWithTotalCount();
			this.isInit = true;

		}

		public nextSong = () => {

			var index = this.getSongIndex(this.pvPlayerViewModel.selectedSong());

			if (index + 1 < this.songsLoaded()) {
				this.pvPlayerViewModel.selectedSong(this.page()[index + 1]);			
			} else {

				if (this.hasMoreSongs()) {
					this.paging.nextPage();
					this.updateResults(false, () => {
						this.pvPlayerViewModel.selectedSong(this.page()[index + 1]);
					});					
				} else {
					this.pvPlayerViewModel.selectedSong(this.page()[0]);					
				}

			}
			
		}

		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(100); // Paging view model
		public pauseNotifications = false;
		public pvServiceIcons: vdb.models.PVServiceIcons;

		public scrollEnd = () => {

			if (this.hasMoreSongs()) {
				this.paging.nextPage();
				this.updateResultsWithoutTotalCount();
			}

		}

		public songsLoaded = ko.computed(() => this.page().length);

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true, callback?: () => void) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults) {
				this.page.removeAll();
				this.paging.page(1);				
			}

			var pagingProperties = this.paging.getPagingProperties(clearResults);
			var services = this.pvPlayerViewModel.autoplay() ? vdb.viewModels.pvs.PVPlayerViewModel.autoplayPVServicesString : "Youtube,SoundCloud,NicoNicoDouga,Bilibili,Vimeo,Piapro,File";

			this.songListRepo.getSongs(this.listId, services, pagingProperties,
				new cls.SongOptionalFields(cls.SongOptionalField.AdditionalNames, cls.SongOptionalField.ThumbUrl),
				this.languageSelection,
				(result: dc.PartialFindResultContract<dc.songs.SongInListContract>) => {

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					_.each(result.items, item => {
						item.song.pvServicesArray = vdb.helpers.PVHelper.pvServicesArrayFromString(item.song.pvServices);
						this.page.push(item);
					});
					
					this.loading(false);

					if (result.items && result.items.length && !this.pvPlayerViewModel.selectedSong())
						this.pvPlayerViewModel.selectedSong(result.items[0]);

					if (callback)
						callback();

				});

		}

	}

}