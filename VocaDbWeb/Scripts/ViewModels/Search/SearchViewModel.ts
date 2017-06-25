
module vdb.viewModels.search {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(
			urlMapper: vdb.UrlMapper,
			entryRepo: rep.EntryRepository, artistRepo: rep.ArtistRepository,
			albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository,
			eventRepo: rep.ReleaseEventRepository,
			tagRepo: rep.TagRepository,
			resourceRepo: rep.ResourceRepository,
			userRepo: rep.UserRepository,
			unknownPictureUrl: string,
			private languageSelection: string, loggedUserId: number, cultureCode: string, searchType: string,
			searchTerm: string,
			tagIds: number[],
			sort: string,
			artistId: number[],
			childTags: boolean,
			childVoicebanks: boolean,
			artistType: string,
			albumType: string, songType: string,
			onlyWithPVs: boolean,
			onlyRatedSongs: boolean,
			since: number,
			minScore: number,
			viewMode: string,
			autoplay: boolean,
			shuffle: boolean,
			pageSize: number,
			pvPlayersFactory: pvs.PVPlayersFactory) {

			this.resourcesManager = new vdb.models.ResourcesManager(resourceRepo, cultureCode);
			this.resources = this.resourcesManager.resources;
			this.tagFilters = new TagFilters(tagRepo, languageSelection);

			if (searchTerm)
				this.searchTerm(searchTerm);

			var isAlbum = searchType === SearchType.Album;
			var isSong = searchType === SearchType.Song;

			this.anythingSearchViewModel = new AnythingSearchViewModel(this, languageSelection, entryRepo);
			this.artistSearchViewModel = new ArtistSearchViewModel(this, languageSelection, artistRepo, loggedUserId, artistType);

			this.albumSearchViewModel = new AlbumSearchViewModel(this, unknownPictureUrl, languageSelection, albumRepo, artistRepo,
				resourceRepo, cultureCode,
				isAlbum ? sort : null,
				isAlbum ? artistId : null,
				isAlbum ? childVoicebanks : null,
				albumType,
				isAlbum ? viewMode : null);

			this.eventSearchViewModel = new EventSearchViewModel(this, models.globalization.ContentLanguagePreference[languageSelection], eventRepo, artistRepo,
				loggedUserId, sort, artistId);

			this.songSearchViewModel = new SongSearchViewModel(this, urlMapper, languageSelection, songRepo, artistRepo, userRepo,
				resourceRepo,
				cultureCode,
				loggedUserId,
				isSong ? sort : null,
				isSong ? artistId : null,
				isSong ? childVoicebanks : null,
				songType,
				onlyWithPVs,
				onlyRatedSongs,
				since,
				isSong ? minScore : null,
				isSong ? viewMode : null,
				autoplay,
				shuffle,
				pvPlayersFactory);

			this.tagSearchViewModel = new TagSearchViewModel(this, models.globalization.ContentLanguagePreference[languageSelection], tagRepo);

			if (tagIds != null || artistId != null || artistType || albumType || songType || onlyWithPVs != null || since || minScore)
				this.showAdvancedFilters(true);

			if (searchType)
				this.searchType(searchType);

			if (tagIds)
				this.tagFilters.addTags(tagIds);

			if (tagIds && childTags)
				this.tagFilters.childTags(childTags);

			if (pageSize)
				this.pageSize(pageSize);

			this.pageSize.subscribe(this.updateResults);
			this.searchTerm.subscribe(this.updateResults);
			this.tagFilters.filters.subscribe(this.updateResults);
			this.draftsOnly.subscribe(this.updateResults);
			this.showTags.subscribe(this.updateResults);

			this.showAnythingSearch = ko.computed(() => this.searchType() === SearchType.Anything);
			this.showArtistSearch = ko.computed(() => this.searchType() === SearchType.Artist);
			this.showAlbumSearch = ko.computed(() => this.searchType() === SearchType.Album);
			this.showSongSearch = ko.computed(() => this.searchType() === SearchType.Song);

			this.searchType.subscribe(val => {

				this.updateResults();
				this.currentSearchType(val);

			});

			resourceRepo.getList(cultureCode, [
				'albumSortRuleNames', 'artistSortRuleNames', 'artistTypeNames',
				'discTypeNames', 'eventCategoryNames', 'eventSortRuleNames',
				'entryTypeNames', 'songSortRuleNames', 'songTypeNames'],
				resources => {
				this.resources(resources);
				this.updateResults();
			});

			tagRepo.getTopTags(languageSelection, models.tags.Tag.commonCategory_Genres, result => {
				this.genreTags(result);
			});

		}

		public albumSearchViewModel: AlbumSearchViewModel;
		public anythingSearchViewModel: AnythingSearchViewModel;
		public artistSearchViewModel: ArtistSearchViewModel;
		public eventSearchViewModel: EventSearchViewModel;
		public songSearchViewModel: SongSearchViewModel;
		public tagSearchViewModel: TagSearchViewModel;

		private currentSearchType = ko.observable(SearchType.Anything);
		public draftsOnly = ko.observable(false);
		public genreTags = ko.observableArray<dc.TagBaseContract>();
		public pageSize = ko.observable(10);
		public resourcesManager: vdb.models.ResourcesManager;
		public resources: KnockoutObservable<dc.ResourcesContract>;
		public showAdvancedFilters = ko.observable(false);
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable(SearchType.Anything);
		public tagFilters: TagFilters;

		public showAnythingSearch: KnockoutComputed<boolean>;
		public showArtistSearch: KnockoutComputed<boolean>;
		public showAlbumSearch: KnockoutComputed<boolean>;
		public showEventSearch = ko.computed(() => this.searchType() === SearchType.ReleaseEvent);
		public showSongSearch: KnockoutComputed<boolean>;
		public showTagSearch = ko.computed(() => this.searchType() === SearchType.Tag);
		public showTagFilter = ko.computed(() => !this.showTagSearch());
		public showTags = ko.observable(false);
		public showDraftsFilter = ko.computed(() => this.searchType() !== SearchType.Tag);

		public isUniversalSearch = ko.computed(() => this.searchType() === SearchType.Anything);

		public currentCategoryViewModel = (): ISearchCategoryBaseViewModel => {
			
			switch (this.searchType()) {
				case SearchType.Anything:
					return this.anythingSearchViewModel;
				case SearchType.Artist:
					return this.artistSearchViewModel;
				case SearchType.Album:
					return this.albumSearchViewModel;
				case SearchType.ReleaseEvent:
					return this.eventSearchViewModel;
				case SearchType.Song:
					return this.songSearchViewModel;
				case SearchType.Tag:
					return this.tagSearchViewModel;
				default:
					return null;
			}

		}

		public updateResults = () => {

			var vm = this.currentCategoryViewModel();

			if (vm != null)
				vm.updateResultsWithTotalCount();
				
		}

	}

	class SearchType {
		public static Anything = "Anything";
		public static Artist = "Artist";
		public static Album = "Album";
		public static ReleaseEvent = "ReleaseEvent";
		public static Song = "Song";
		public static Tag = "Tag";
	}

}