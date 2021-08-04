import EntryContract from '@DataContracts/EntryContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryRepository from '@Repositories/EntryRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import { computed, makeObservable } from 'mobx';

import { ICommonSearchStore } from './CommonSearchStore';
import SearchCategoryBaseStore from './SearchCategoryBaseStore';

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
}
