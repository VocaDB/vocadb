import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagRepository from '@Repositories/TagRepository';
import GlobalValues from '@Shared/GlobalValues';
import { makeObservable, observable, reaction } from 'mobx';

import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';

// Corresponds to the TagSortRule enum in C#.
export enum TagSortRule {
	Nothing = 'Nothing',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	UsageCount = 'UsageCount',
}

export default class TagSearchStore extends SearchCategoryBaseStore<TagApiContract> {
	@observable public allowAliases = false;
	@observable public categoryName?: string;
	@observable public sort = TagSortRule.Name;

	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly tagRepo: TagRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);

		reaction(() => this.allowAliases, this.updateResultsWithTotalCount);
		reaction(() => this.categoryName, this.updateResultsWithTotalCount);
		reaction(() => this.sort, this.updateResultsWithTotalCount);
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<TagApiContract>> => {
		return this.tagRepo.getList({
			queryParams: {
				start: pagingProperties.start,
				maxResults: pagingProperties.maxEntries,
				getTotalCount: pagingProperties.getTotalCount,
				lang: this.values.languagePreference,
				query: searchTerm,
				sort: this.sort,
				allowAliases: this.allowAliases,
				categoryName: this.categoryName,
				fields: 'AdditionalNames,MainPicture',
			},
		});
	};
}
