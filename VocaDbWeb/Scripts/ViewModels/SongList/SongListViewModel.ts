
//module vdb.viewModels.songList {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongListViewModel {

		constructor(
			urlMapper: UrlMapper,
			private songListRepo: rep.SongListRepository,
			private songRepo: rep.SongRepository,
			private userRepo: rep.UserRepository,
			private artistRepo: rep.ArtistRepository,
			resourceRepo: rep.ResourceRepository,
			defaultSortRuleName: string,
			latestComments: dc.CommentContract[],
			loggedUserId: number,
			private languageSelection: cls.globalization.ContentLanguagePreference,
			cultureCode: string,
			private listId: number,
			tagUsages: dc.tags.TagUsageForApiContract[],
			pvPlayersFactory: pvs.PVPlayersFactory,
			canDeleteAllComments: boolean) {

			this.artistFilters = new vdb.viewModels.search.ArtistFilters(this.artistRepo, false);
			this.comments = new EditableCommentsViewModel(songListRepo.getComments(), listId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);

			this.resourceManager = new cls.ResourcesManager(resourceRepo, cultureCode);
			this.resourceManager.loadResources(null, "songSortRuleNames");
			this.sortName = ko.computed(() => {

				if (this.sort() === "")
					return defaultSortRuleName;

				return this.resourceManager.resources().songSortRuleNames != null ? this.resourceManager.resources().songSortRuleNames[this.sort()] : "";

			});

			this.tagIds = ko.computed(() => _.map(this.tags(), t => t.id));

			// TODO
			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, pvPlayersFactory);
			var playListRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForSongListAdapter(songListRepo, listId,
				this.query, this.songType, this.tagIds, this.childTags, this.artistFilters.artistIds,
				this.artistFilters.artistParticipationStatus, this.artistFilters.childVoicebanks, this.advancedFilters.filters,
				this.sort);
			this.playlistViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, playListRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel, languageSelection);
			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.artists.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.childVoicebanks.subscribe(this.updateResultsWithTotalCount);
			this.childTags.subscribe(this.updateResultsWithTotalCount);
			this.showTags.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepo.getSongListTagSelections(this.listId, callback),
				saveTagSelections: tags => userRepo.updateSongListTags(this.listId, tags, this.tagUsages.updateTagUsages)
			}, cls.EntryType.SongList);

			this.tags.subscribe(this.updateResultsWithTotalCount);

			this.tagUsages = new tags.TagListViewModel(tagUsages);

			this.sort.subscribe(() => this.updateCurrentMode(true));
			this.playlistMode.subscribe(() => this.updateCurrentMode(true));
			this.query.subscribe(() => this.updateCurrentMode(true));

			this.updateResultsWithTotalCount();

		}

		public addTag = (tag: dc.TagBaseContract) => this.tags.push(new viewModels.search.TagFilter(tag.id, tag.name, tag.urlSlug));

		public advancedFilters = new viewModels.search.AdvancedSearchFilters();
		public artistFilters: vdb.viewModels.search.ArtistFilters;
		public childTags = ko.observable(false);
		public comments: EditableCommentsViewModel;

		public loading = ko.observable(true); // Currently loading for data

		public mapTagUrl = (tagUsage: vdb.dataContracts.tags.TagUsageForApiContract) => {
			return utils.EntryUrlMapper.details_tag(tagUsage.tag.id, tagUsage.tag.urlSlug);
		}

		public page = ko.observableArray<dc.songs.SongInListContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public playlistMode = ko.observable(false);
		public playlistViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;
		public query = ko.observable("");
		private resourceManager: cls.ResourcesManager;
		public showAdvancedFilters = ko.observable(false);
		public showTags = ko.observable(false);
		public sort = ko.observable("");
		public sortName: KnockoutComputed<string>;
		public songType = ko.observable(models.songs.SongType[models.songs.SongType.Unspecified]);
		public tagsEditViewModel: tags.TagsEditViewModel;
		public tags = ko.observableArray<viewModels.search.TagFilter>();
		public tagIds: KnockoutComputed<number[]>;
		public tagUsages: tags.TagListViewModel;

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		private updateCurrentMode = (clearResults: boolean) => {
			
			if (this.playlistMode()) {
				this.playlistViewModel.updateResultsWithTotalCount();				
			} else {
				this.updateResults(clearResults);
			}

		}

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

			this.songListRepo.getSongs(this.listId, this.query(),
				this.songType() !== cls.songs.SongType[cls.songs.SongType.Unspecified] ? this.songType() : null,
				this.tagIds(),
				this.childTags(),
				this.artistFilters.artistIds(),
				this.artistFilters.artistParticipationStatus(),
				this.artistFilters.childVoicebanks(),
				this.advancedFilters.filters(),
				null, pagingProperties,
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

//}