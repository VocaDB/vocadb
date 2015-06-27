
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

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.artistType.subscribe(this.updateResultsWithTotalCount);
			this.onlyFollowedByMe.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tags, status, callback) => {

				this.artistRepo.getList(pagingProperties, lang, searchTerm, this.sort(),
					this.artistType() != cls.artists.ArtistType[cls.artists.ArtistType.Unknown] ? this.artistType() : null,
					tags,
					this.onlyFollowedByMe() ? this.loggedUserId : null,
					this.fields(), status, callback);

			}

		}

		public artistType = ko.observable("Unknown");
		public onlyFollowedByMe = ko.observable(false);
		public showTags = ko.observable(false);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.searchViewModel.resources().artistSortRuleNames != null ? this.searchViewModel.resources().artistSortRuleNames[this.sort()] : "");

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "MainPicture,Tags" : "MainPicture");

	}

}