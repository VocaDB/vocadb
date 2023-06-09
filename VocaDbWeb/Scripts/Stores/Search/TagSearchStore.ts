import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { TagOptionalField, TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { includesAny, StateChangeEvent } from '@/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

// Corresponds to the TagSortRule enum in C#.
export enum TagSortRule {
	Nothing = 'Nothing',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	UsageCount = 'UsageCount',
}

export interface TagSearchRouteParams {
	categoryName?: string;
	filter?: string;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Tag;
	sort?: TagSortRule;
}

const clearResultsByQueryKeys: (keyof TagSearchRouteParams)[] = [
	'pageSize',
	'filter',
	'searchType',

	// TODO: allowAliases
	'categoryName',
	'sort',
];

export class TagSearchStore extends SearchCategoryBaseStore<
	TagSearchRouteParams,
	TagApiContract
> {
	@observable allowAliases = false;
	@observable categoryName?: string;
	@observable sort = TagSortRule.Name;

	constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly tagRepo: TagRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
	}

	loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<TagApiContract>> => {
		return this.tagRepo.getList({
			queryParams: {
				start: pagingProperties.start,
				maxResults: pagingProperties.maxEntries,
				getTotalCount: pagingProperties.getTotalCount,
				lang: this.values.languagePreference,
				query: this.searchTerm,
				sort: this.sort,
				allowAliases: this.allowAliases,
				categoryName: this.categoryName,
				fields: [
					TagOptionalField.AdditionalNames,
					TagOptionalField.MainPicture,
				],
			},
		});
	};

	@computed.struct get locationState(): TagSearchRouteParams {
		return {
			searchType: SearchType.Tag,
			categoryName: this.categoryName,
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
		};
	}
	set locationState(value: TagSearchRouteParams) {
		this.categoryName = value.categoryName;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.sort = value.sort ?? TagSortRule.Name;
	}

	onLocationStateChange = (
		event: StateChangeEvent<TagSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
