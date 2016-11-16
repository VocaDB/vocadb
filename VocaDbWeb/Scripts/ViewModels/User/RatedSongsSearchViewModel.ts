
module vdb.viewModels.user {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class RatedSongsSearchViewModel {
		
		constructor(
			urlMapper: UrlMapper,
			private userRepo: rep.UserRepository, private artistRepo: rep.ArtistRepository,
			private songRepo: rep.SongRepository,
			private resourceRepo: rep.ResourceRepository,
			tagRepo: rep.TagRepository,
			private languageSelection: string, private loggedUserId: number, private cultureCode: string,
			sort: string, groupByRating: boolean,
			pvPlayersFactory: pvs.PVPlayersFactory,
			initialize = true,
			artistId?: number,
			childVoicebanks?: boolean) {	

			this.artistFilters = new viewModels.search.ArtistFilters(artistRepo, childVoicebanks);

			if (artistId)
				this.artistFilters.selectArtist(artistId);

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			if (sort)
				this.sort(sort);

			if (groupByRating != null)
				this.groupByRating(groupByRating);

			this.tagFilters = new viewModels.search.TagFilters(tagRepo, languageSelection);

			this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.groupByRating.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.rating.subscribe(this.updateResultsWithTotalCount);
			this.searchTerm.subscribe(this.updateResultsWithTotalCount);
			this.showTags.subscribe(this.updateResultsWithoutTotalCount);
			this.songListId.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithoutTotalCount);
			this.tagFilters.tags.subscribe(this.updateResultsWithTotalCount);
			this.viewMode.subscribe(this.updateResultsWithTotalCount);

			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, pvPlayersFactory);
			var songsRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForRatedSongsAdapter(userRepo, loggedUserId, this.searchTerm, this.sort,
				this.tagFilters.tagIds, this.artistFilters.artistIds, this.artistFilters.childVoicebanks,
				this.rating, this.songListId, this.advancedFilters.filters, this.groupByRating, ko.observable("AdditionalNames,ThumbUrl"));
			this.playListViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, songsRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel,
				cls.globalization.ContentLanguagePreference[languageSelection]);

			if (initialize)
				this.init();

		}

		public advancedFilters = new viewModels.search.AdvancedSearchFilters;
		public artistFilters: viewModels.search.ArtistFilters;
		public groupByRating = ko.observable(true);
		public isInit = false;
		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<IRatedSongSearchItem>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public playListViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;
		public rating = ko.observable("Nothing");
		public resources = ko.observable<any>();
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public showTags = ko.observable(false);
		public songListId = ko.observable<number>(undefined);
		public songLists = ko.observableArray<dc.SongListBaseContract>([]);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.resources() != null ? (this.resources().user_ratedSongForUserSortRuleNames[this.sort()]
			|| this.resources().songSortRuleNames[this.sort()]) : "");
		public tagFilters: viewModels.search.TagFilters;
		public viewMode = ko.observable("Details");

		public fields = ko.computed(() => {
			return "AdditionalNames,ThumbUrl" + (this.showTags() ? ",Tags" : "");
		});

		public formatDate = (dateStr: string) => {
			return moment(dateStr).format('l');
		}

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		public init = () => {

			if (this.isInit)
				return;

			this.userRepo.getSongLists(this.loggedUserId, null, { start: 0, maxEntries: 50, getTotalCount: false }, "Name", null, songLists => this.songLists(songLists.items));

			this.resourceRepo.getList(this.cultureCode, ['songSortRuleNames', 'user_ratedSongForUserSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResultsWithTotalCount();
				this.isInit = true;
			});

		};

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

			if (this.viewMode() === "PlayList") {
				this.playListViewModel.updateResultsWithTotalCount(() => {
					this.pauseNotifications = false;
					this.loading(false);					
				});
				return;
			}

			this.userRepo.getRatedSongsList(this.loggedUserId, pagingProperties, this.languageSelection, this.searchTerm(),
				this.tagFilters.tagIds(),
				this.artistFilters.artistIds(),
				this.artistFilters.childVoicebanks(),
				this.rating(),
				this.songListId(),
				this.advancedFilters.filters(),
				this.groupByRating(),
				null,
				this.fields(),
				this.sort(),
				(result: dc.PartialFindResultContract<dc.RatedSongForUserForApiContract>) => {

					var songs: IRatedSongSearchItem[] = [];

					_.each(result.items, (item) => {

						var song: IRatedSongSearchItem = item.song;

						song.rating = item.rating;

						if (song.pvServices && song.pvServices !== 'Nothing') {
							song.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
							song.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
						} else {
							song.previewViewModel = null;
						}

						songs.push(song);

					});

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(songs);
					this.loading(false);

				});

		}

	}

	export interface IRatedSongSearchItem extends dc.SongApiContract {

		previewViewModel?: SongWithPreviewViewModel;

		rating?: string;

	}

}