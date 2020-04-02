import EntryContract from '../../DataContracts/EntryContract';
import EntryRepository from '../../Repositories/EntryRepository';
import EntryType from '../../Models/EntryType';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import SearchCategoryBaseViewModel from './SearchCategoryBaseViewModel';
import SearchViewModel from './SearchViewModel';

	export default class AnythingSearchViewModel extends SearchCategoryBaseViewModel<EntryContract> {

		constructor(searchViewModel: SearchViewModel, lang: string, private entryRepo: EntryRepository) {

			super(searchViewModel);

			this.loadResults = (pagingProperties, searchTerm, tags, childTags, status, callback) =>
				this.entryRepo.getList(pagingProperties, lang, searchTerm, tags, childTags, this.fields(), status, callback);

		}

		public entryCategoryName = (entry: EntryContract) => {

			switch (EntryType[entry.entryType]) {
				case EntryType.Artist:
					return this.searchViewModel.resources().artistTypeNames[entry.artistType];
				case EntryType.Album:
					return this.searchViewModel.resources().discTypeNames[entry.discType];
				case EntryType.ReleaseEvent:
					return this.searchViewModel.resources().eventCategoryNames[entry.eventCategory];
				case EntryType.Song:
					return this.searchViewModel.resources().songTypeNames[entry.songType];
			}

			return null;

		}

		public entryUrl = (entry: EntryContract) => {

			return EntryUrlMapper.details(entry.entryType, entry.id);

		}

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "AdditionalNames,MainPicture,Tags" : "AdditionalNames,MainPicture");

	}