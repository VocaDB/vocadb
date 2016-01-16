
module vdb.viewModels.search {

	import dc = vdb.dataContracts;

	export class TagSearchViewModel extends SearchCategoryBaseViewModel<dc.TagApiContract> {

		constructor(searchViewModel: SearchViewModel,
			lang: string,
			private tagRepo: rep.TagRepository) {

			super(searchViewModel);

			this.allowAliases.subscribe(this.updateResultsWithTotalCount);
			this.categoryName.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.tagRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.allowAliases(), this.categoryName(), callback);

			}

		}

		public allowAliases = ko.observable(false);
		public categoryName = ko.observable("");
		public sort = ko.observable("Name");

	}

}