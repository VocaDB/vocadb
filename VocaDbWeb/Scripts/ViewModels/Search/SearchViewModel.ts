
module vdb.viewModels.search {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SearchViewModel {

		constructor(
			urlMapper: vdb.UrlMapper,
			entryRepo: rep.EntryRepository, artistRepo: rep.ArtistRepository,
			albumRepo: rep.AlbumRepository, songRepo: rep.SongRepository,
			tagRepo: rep.TagRepository,
			resourceRepo: rep.ResourceRepository,
			userRepo: rep.UserRepository,
			unknownPictureUrl: string,
			languageSelection: string, loggedUserId: number, cultureCode: string, searchType: string,
			searchTerm: string,
			tag: string,
			sort: string,
			artistId: number,
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
			pvPlayersFactory: pvs.PVPlayersFactory) {

			this.resourcesManager = new vdb.models.ResourcesManager(resourceRepo, cultureCode);
			this.resources = this.resourcesManager.resources;

			if (searchTerm)
				this.searchTerm(searchTerm);

			var isAlbum = searchType === "Album";
			var isSong = searchType === "Song";

			this.anythingSearchViewModel = new AnythingSearchViewModel(this, languageSelection, entryRepo);
			this.artistSearchViewModel = new ArtistSearchViewModel(this, languageSelection, artistRepo, loggedUserId, artistType);

			this.albumSearchViewModel = new AlbumSearchViewModel(this, unknownPictureUrl, languageSelection, albumRepo, artistRepo,
				resourceRepo, cultureCode,
				isAlbum ? sort : null,
				isAlbum ? artistId : null,
				isAlbum ? childVoicebanks : null,
				albumType,
				isAlbum ? viewMode : null);

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

			this.tagSearchViewModel = new TagSearchViewModel(this, tagRepo);

			if (tag || artistId != null || artistType || albumType || songType || onlyWithPVs != null || since || minScore)
				this.showAdvancedFilters(true);

			if (searchType)
				this.searchType(searchType);

			if (tag)
				this.tag(tag);

			this.pageSize.subscribe(this.updateResults);
			this.searchTerm.subscribe(this.updateResults);
			this.tag.subscribe(this.updateResults);
			this.draftsOnly.subscribe(this.updateResults);
			this.showTags.subscribe(this.updateResults);

			this.showAnythingSearch = ko.computed(() => this.searchType() == 'Anything');
			this.showArtistSearch = ko.computed(() => this.searchType() == 'Artist');
			this.showAlbumSearch = ko.computed(() => this.searchType() == 'Album');
			this.showSongSearch = ko.computed(() => this.searchType() == 'Song');

			this.searchType.subscribe(val => {

				this.updateResults();
				this.currentSearchType(val);

			});

			resourceRepo.getList(cultureCode, ['albumSortRuleNames', 'artistSortRuleNames', 'artistTypeNames', 'discTypeNames', 'entryTypeNames', 'songSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResults();
			});

			tagRepo.getTopTags("Genres", result => {
				this.genreTags(result);
			});

		}

		public albumSearchViewModel: AlbumSearchViewModel;
		public anythingSearchViewModel: AnythingSearchViewModel;
		public artistSearchViewModel: ArtistSearchViewModel;
		public songSearchViewModel: SongSearchViewModel;
		public tagSearchViewModel: TagSearchViewModel;

		private currentSearchType = ko.observable("Anything");
		public draftsOnly = ko.observable(false);
		public genreTags = ko.observableArray<string>();
		public pageSize = ko.observable(10);
		public resourcesManager: vdb.models.ResourcesManager;
		public resources: KnockoutObservable<dc.ResourcesContract>;
		//public resources = ko.observable<dc.ResourcesContract>();
		public showAdvancedFilters = ko.observable(false);
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public searchType = ko.observable("Anything");
		public tag = ko.observable("");

		public showAnythingSearch: KnockoutComputed<boolean>;
		public showArtistSearch: KnockoutComputed<boolean>;
		public showAlbumSearch: KnockoutComputed<boolean>;
		public showSongSearch: KnockoutComputed<boolean>;
		public showTagSearch = ko.computed(() => this.searchType() == 'Tag');
		public showTagFilter = ko.computed(() => !this.showTagSearch());
		public showTags = ko.observable(false);
		public showDraftsFilter = ko.computed(() => this.searchType() != 'Tag');

		public isUniversalSearch = ko.computed(() => this.searchType() == 'Anything');

		public currentCategoryViewModel = (): ISearchCategoryBaseViewModel => {
			
			switch (this.searchType()) {
				case 'Anything':
					return this.anythingSearchViewModel;
				case 'Artist':
					return this.artistSearchViewModel;
				case 'Album':
					return this.albumSearchViewModel;
				case 'Song':
					return this.songSearchViewModel;
				case 'Tag':
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

}