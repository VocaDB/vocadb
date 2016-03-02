
module vdb.viewModels.search {

	import dc = vdb.dataContracts;

	export class TagSearchViewModel extends SearchCategoryBaseViewModel<dc.TagApiContract> {

		constructor(searchViewModel: SearchViewModel,
			lang: vdb.models.globalization.ContentLanguagePreference,
			private tagRepo: rep.TagRepository) {

			super(searchViewModel);

			this.allowAliases.subscribe(this.updateResultsWithTotalCount);
			this.categoryName.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.tagRepo.getList({ start: pagingProperties.start, maxResults: pagingProperties.maxEntries, getTotalCount: pagingProperties.getTotalCount, lang: lang, query: searchTerm, sort: this.sort(), allowAliases: this.allowAliases(), categoryName: this.categoryName(), fields: "AdditionalNames,MainPicture" },
					callback);

			}

		}

		public allowAliases = ko.observable(false);
		public categoryName = ko.observable("");
		public sort = ko.observable("Name");

	}

}