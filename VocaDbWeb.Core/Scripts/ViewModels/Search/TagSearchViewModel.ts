import ContentLanguagePreference from '../../Models/Globalization/ContentLanguagePreference';
import SearchCategoryBaseViewModel from './SearchCategoryBaseViewModel';
import SearchViewModel from './SearchViewModel';
import TagApiContract from '../../DataContracts/Tag/TagApiContract';
import TagRepository from '../../Repositories/TagRepository';

	export default class TagSearchViewModel extends SearchCategoryBaseViewModel<TagApiContract> {

		constructor(searchViewModel: SearchViewModel,
			lang: ContentLanguagePreference,
			private tagRepo: TagRepository) {

			super(searchViewModel);

			this.allowAliases.subscribe(this.updateResultsWithTotalCount);
			this.categoryName.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, childTags, status, callback) => {

				this.tagRepo.getList({
					start: pagingProperties.start, maxResults: pagingProperties.maxEntries, getTotalCount: pagingProperties.getTotalCount,
					lang: lang, query: searchTerm, sort: this.sort(), allowAliases: this.allowAliases(), categoryName: this.categoryName(), fields: "AdditionalNames,MainPicture"
				}, callback);

			}

		}

		public allowAliases = ko.observable(false);
		public categoryName = ko.observable("");
		public sort = ko.observable("Name");

	}