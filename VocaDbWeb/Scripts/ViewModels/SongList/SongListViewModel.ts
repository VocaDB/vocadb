
module vdb.viewModels.songList {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongListViewModel {

		constructor(
			urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository,
			private userRepo: rep.UserRepository, 
			resourceRepo: rep.ResourceRepository,
			defaultSortRuleName: string,
			private languageSelection: cls.globalization.ContentLanguagePreference,
			cultureCode: string,
			private listId: number,
			pvPlayersFactory: pvs.PVPlayersFactory) {

			this.resourceManager = new cls.ResourcesManager(resourceRepo, cultureCode);
			this.resourceManager.loadResources(null, "songSortRuleNames");
			this.sortName = ko.computed(() => {

				if (this.sort() === "")
					return defaultSortRuleName;

				return this.resourceManager.resources().songSortRuleNames != null ? this.resourceManager.resources().songSortRuleNames[this.sort()] : "";

			});

			// TODO
			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, pvPlayersFactory);
			var playListRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForSongListAdapter(songListRepo, listId, this.sort);
			this.playlistViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, playListRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel, languageSelection);
			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.showTags.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);

			this.sort.subscribe(() => {
				this.updateResultsWithTotalCount();
				if (this.playlistMode())
					this.playlistViewModel.updateResultsWithTotalCount();
			});

			this.playlistMode.subscribe(mode => {
				if (mode)
					this.playlistViewModel.init();
			});

			this.updateResultsWithTotalCount();

		}

		public loading = ko.observable(true); // Currently loading for data

		public mapTagUrl = (tagName: string) => {
			return utils.EntryUrlMapper.details_tag_byName(tagName);
		}

		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public playlistMode = ko.observable(false);
		public playlistViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;
		private resourceManager: cls.ResourcesManager;
		public showTags = ko.observable(false);
		public sort = ko.observable("");
		public sortName: KnockoutComputed<string>;

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

			var fields = [cls.SongOptionalField.AdditionalNames, cls.SongOptionalField.ThumbUrl];

			if (this.showTags())
				fields.push(cls.SongOptionalField.Tags);

			this.songListRepo.getSongs(this.listId, null, pagingProperties,
				new cls.SongOptionalFields(fields),
				this.sort(),
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