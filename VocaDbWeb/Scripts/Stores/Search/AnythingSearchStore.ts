import EntryContract from '@DataContracts/EntryContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryRepository from '@Repositories/EntryRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import { computed, makeObservable } from 'mobx';

import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import SearchQueryParams from './SearchQueryParams';
import { SearchType } from './SearchStore';

export default class AnythingSearchStore extends SearchCategoryBaseStore<EntryContract> {
	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly entryRepo: EntryRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
	}

	@computed public get queryParams(): SearchQueryParams {
		return {
			searchType: SearchType.Anything,
			filter: this.searchTerm,
			tagId: this.tagIds,
			childTags: this.childTags,
			pageSize: this.pageSize,
		};
	}
	public set queryParams(value: SearchQueryParams) {
		if (value.searchType !== SearchType.Anything) return;

		this.searchTerm = value.filter ?? '';
		this.tagIds = value.tagId ?? [];
		this.childTags = value.childTags ?? false;
		this.pageSize = value.pageSize ?? 10;
	}

	@computed public get fields(): string {
		return this.showTags
			? 'AdditionalNames,MainPicture,Tags'
			: 'AdditionalNames,MainPicture';
	}

	public loadResults = (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	): Promise<PartialFindResultContract<EntryContract>> => {
		return this.entryRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: searchTerm,
			tags: tags,
			childTags: childTags,
			fields: this.fields,
			status: status,
		});
	};

	public entryUrl = (entry: EntryContract): string => {
		return EntryUrlMapper.details(entry.entryType, entry.id);
	};
}
