import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import TagRepository from '@Repositories/TagRepository';
import vdb from '@Shared/VdbStatic';
import ko from 'knockout';

import SearchCategoryBaseViewModel from './SearchCategoryBaseViewModel';
import SearchViewModel from './SearchViewModel';

export default class TagSearchViewModel extends SearchCategoryBaseViewModel<TagApiContract> {
	public constructor(
		searchViewModel: SearchViewModel,
		lang: ContentLanguagePreference,
		private tagRepo: TagRepository,
	) {
		super(searchViewModel);

		this.allowAliases.subscribe(this.updateResultsWithTotalCount);
		this.categoryName.subscribe(this.updateResultsWithTotalCount);
		this.sort.subscribe(this.updateResultsWithTotalCount);

		this.loadResults = (
			pagingProperties,
			searchTerm,
			tag,
			childTags,
			status,
		): Promise<PartialFindResultContract<TagApiContract>> =>
			this.tagRepo.getList({
				queryParams: {
					start: pagingProperties.start,
					maxResults: pagingProperties.maxEntries,
					getTotalCount: pagingProperties.getTotalCount,
					lang: vdb.values.languagePreference,
					query: searchTerm,
					sort: this.sort(),
					allowAliases: this.allowAliases(),
					categoryName: this.categoryName(),
					fields: 'AdditionalNames,MainPicture',
				},
			});
	}

	public allowAliases = ko.observable(false);
	public categoryName = ko.observable('');
	public sort = ko.observable('Name');
}
