import EntryContract from '@DataContracts/EntryContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryRepository from '@Repositories/EntryRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { computed, makeObservable } from 'mobx';

import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';
import { SearchRouteParams, SearchType } from './SearchStore';

export interface AnythingSearchRouteParams {
	childTags?: boolean;
	draftsOnly?: boolean;
	filter?: string;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Anything;
	tag?: string;
	tagId?: number[];
}

export default class AnythingSearchStore extends SearchCategoryBaseStore<EntryContract> {
	public constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly entryRepo: EntryRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
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

	@computed public get routeParams(): SearchRouteParams {
		return {
			searchType: SearchType.Anything,
			childTags: this.childTags || undefined,
			draftsOnly: this.draftsOnly || undefined,
			filter: this.searchTerm || undefined,
			page: this.paging.page,
			pageSize: this.pageSize,
			tagId: this.tagIds,
		};
	}
	public set routeParams(value: SearchRouteParams) {
		if (value.searchType !== SearchType.Anything) return;

		this.childTags = value.childTags ?? false;
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.pageSize = value.pageSize ?? 10;
		this.tagIds = value.tagId ?? [];
	}

	public shouldClearResults = (value: SearchRouteParams): boolean => {
		if (value.searchType !== SearchType.Anything) return true;

		const routeParams = this.routeParams;
		if (routeParams.searchType !== SearchType.Anything) return true;

		if (!_.isEqual(value.tagId, routeParams.tagId)) return true;
		if (value.draftsOnly !== routeParams.draftsOnly) return true;

		return false;
	};
}
