
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class AlbumSearchViewModel extends SearchCategoryBaseViewModel<dc.AlbumContract> {

		constructor(searchViewModel: SearchViewModel,
			private unknownPictureUrl: string,
			lang: string, private albumRepo: rep.AlbumRepository,
			private artistRepo: rep.ArtistRepository,
			resourceRep: rep.ResourceRepository,
			cultureCode: string,
			sort: string,
			artistId: number[],
			childVoicebanks: boolean,
			albumType: string,
			viewMode: string) {

			super(searchViewModel);

			if (searchViewModel) {
				this.resourceManager = searchViewModel.resourcesManager;
			} else {
				this.resourceManager = new cls.ResourcesManager(resourceRep, cultureCode);
				this.resourceManager.loadResources(null, "albumSortRuleNames", "discTypeNames");
			}

			this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters = new ArtistFilters(this.artistRepo, childVoicebanks);
			this.artistFilters.selectArtists(artistId);

			this.albumType = ko.observable(albumType || "Unknown");
			this.sort = ko.observable(sort || "Name");
			this.viewMode = ko.observable(viewMode || "Details");

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.albumType.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.filters.subscribe(this.updateResultsWithTotalCount);

			this.sortName = ko.computed(() => {
				return this.resourceManager.resources().albumSortRuleNames != null ? this.resourceManager.resources().albumSortRuleNames[this.sort()] : "";
			});

			this.loadResults = (pagingProperties, searchTerm, tags, childTags, status, callback) => {

				var artistIds = this.artistFilters.artistIds();

				this.albumRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.albumType(), tags, childTags, artistIds,
					this.artistFilters.artistParticipationStatus(), this.artistFilters.childVoicebanks(), this.fields(), status, false,
					this.advancedFilters.filters(), callback);

			}

		}

		public albumType: KnockoutObservable<string>;
		public artistFilters: ArtistFilters;
		private resourceManager: cls.ResourcesManager;
		public sort: KnockoutObservable<string>;
		public sortName: KnockoutComputed<string>;
		public viewMode: KnockoutObservable<string>;

		public discTypeName = (discTypeStr: string) => this.resourceManager.resources().discTypeNames != null ? this.resourceManager.resources().discTypeNames[discTypeStr] : "";

		public fields = ko.computed(() => this.showTags() ? "AdditionalNames,MainPicture,Tags" : "AdditionalNames,MainPicture");

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => {
				return {
					enabled: (Math.round(album.ratingAverage) >= rating)
				}
			});
			return ratings;

		};

	}

}