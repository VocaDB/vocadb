
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ArtistSearchViewModel extends SearchCategoryBaseViewModel<dc.ArtistApiContract> {

		constructor(searchViewModel: SearchViewModel, lang: string,
			private artistRepo: rep.ArtistRepository,
			private loggedUserId: number,
			artistType: string) {

			super(searchViewModel);

			if (artistType)
				this.artistType(artistType);

			this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.artistType.subscribe(this.updateResultsWithTotalCount);
			this.onlyFollowedByMe.subscribe(this.updateResultsWithTotalCount);
			this.onlyRootVoicebanks.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tags, childTags, status, callback) => {

				this.artistRepo.getList(pagingProperties, lang, searchTerm, this.sort(),
					this.artistType() != cls.artists.ArtistType[cls.artists.ArtistType.Unknown] ? this.artistType() : null,
					!this.onlyRootVoicebanks(),
					tags, childTags,
					this.onlyFollowedByMe() ? this.loggedUserId : null,
					this.fields(), status, this.advancedFilters.filters(),
					callback);

			}

		}

		public artistType = ko.observable("Unknown");
		public onlyFollowedByMe = ko.observable(false);
		public onlyRootVoicebanks = ko.observable(false);
		public showTags = ko.observable(false);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.searchViewModel.resources().artistSortRuleNames != null ? this.searchViewModel.resources().artistSortRuleNames[this.sort()] : "");

		public canHaveChildVoicebanks = ko.computed(() => helpers.ArtistHelper.canHaveChildVoicebanks(cls.artists.ArtistType[this.artistType()]));

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "AdditionalNames,MainPicture,Tags" : "AdditionalNames,MainPicture");

	}

}