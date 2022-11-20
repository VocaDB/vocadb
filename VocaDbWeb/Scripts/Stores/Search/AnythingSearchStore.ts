import { EntryContract } from '@/DataContracts/EntryContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import {
	EntryOptionalField,
	EntryRepository,
} from '@/Repositories/EntryRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import { SearchType } from '@/Stores/Search/SearchStore';
import { includesAny, StateChangeEvent } from '@vocadb/route-sphere';
import { computed, makeObservable } from 'mobx';

export interface AnythingSearchRouteParams {
	childTags?: boolean;
	draftsOnly?: boolean;
	filter?: string;
	page?: number;
	pageSize?: number;
	searchType?: SearchType.Anything;
	tag?: string;
	tagId?: number | number[];
}

const clearResultsByQueryKeys: (keyof AnythingSearchRouteParams)[] = [
	'pageSize',
	'filter',
	'tagId',
	'draftsOnly',
	'searchType',
];

export class AnythingSearchStore extends SearchCategoryBaseStore<
	AnythingSearchRouteParams,
	EntryContract
> {
	constructor(
		commonSearchStore: ICommonSearchStore,
		private readonly values: GlobalValues,
		private readonly entryRepo: EntryRepository,
	) {
		super(commonSearchStore);

		makeObservable(this);
	}

	@computed get fields(): EntryOptionalField[] {
		return this.showTags
			? [
					EntryOptionalField.AdditionalNames,
					EntryOptionalField.MainPicture,
					EntryOptionalField.Tags,
			  ]
			: [EntryOptionalField.AdditionalNames, EntryOptionalField.MainPicture];
	}

	loadResults = (
		pagingProperties: PagingProperties,
	): Promise<PartialFindResultContract<EntryContract>> => {
		return this.entryRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			tags: this.tagIds,
			childTags: this.childTags,
			fields: this.fields,
			status: this.draftsOnly ? 'Draft' : undefined,
		});
	};

	entryUrl = (entry: EntryContract): string => {
		return EntryUrlMapper.details(entry.entryType, entry.id);
	};

	@computed.struct get locationState(): AnythingSearchRouteParams {
		return {
			searchType: SearchType.Anything,
			childTags: this.childTags,
			draftsOnly: this.draftsOnly,
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			tagId: this.tagIds,
		};
	}
	set locationState(value: AnythingSearchRouteParams) {
		this.childTags = value.childTags ?? false;
		this.draftsOnly = value.draftsOnly ?? false;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}

	onLocationStateChange = (
		event: StateChangeEvent<AnythingSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
