
module vdb.viewModels.search {

	import dc = vdb.dataContracts;

	export class AnythingSearchViewModel extends SearchCategoryBaseViewModel<dc.EntryContract> {

		constructor(searchViewModel: SearchViewModel, lang: string, private entryRepo: rep.EntryRepository) {

			super(searchViewModel);

			this.loadResults = (pagingProperties, searchTerm, tags, status, callback) =>
				this.entryRepo.getList(pagingProperties, lang, searchTerm, tags, this.fields(), status, callback);

		}

		public entryUrl = (entry: dc.EntryContract) => {

			return vdb.utils.EntryUrlMapper.details(entry.entryType, entry.id);

		}

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "MainPicture,Tags" : "MainPicture");

	}

}