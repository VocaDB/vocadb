
//module vdb.viewModels.search {

	import dc = vdb.dataContracts;

	export class AnythingSearchViewModel extends SearchCategoryBaseViewModel<dc.EntryContract> {

		constructor(searchViewModel: SearchViewModel, lang: string, private entryRepo: rep.EntryRepository) {

			super(searchViewModel);

			this.loadResults = (pagingProperties, searchTerm, tags, childTags, status, callback) =>
				this.entryRepo.getList(pagingProperties, lang, searchTerm, tags, childTags, this.fields(), status, callback);

		}

		public entryCategoryName = (entry: dc.EntryContract) => {

			switch (models.EntryType[entry.entryType]) {
				case models.EntryType.Artist:
					return this.searchViewModel.resources().artistTypeNames[entry.artistType];
				case models.EntryType.Album:
					return this.searchViewModel.resources().discTypeNames[entry.discType];
				case models.EntryType.ReleaseEvent:
					return this.searchViewModel.resources().eventCategoryNames[entry.eventCategory];
				case models.EntryType.Song:
					return this.searchViewModel.resources().songTypeNames[entry.songType];
			}

			return null;

		}

		public entryUrl = (entry: dc.EntryContract) => {

			return vdb.utils.EntryUrlMapper.details(entry.entryType, entry.id);

		}

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "AdditionalNames,MainPicture,Tags" : "AdditionalNames,MainPicture");

	}

//}