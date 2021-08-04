import TagRepository from '@Repositories/TagRepository';
import GlobalValues from '@Shared/GlobalValues';
import { makeObservable, observable } from 'mobx';

import TagFilters from './TagFilters';

export interface ICommonSearchStore {
	draftsOnly: boolean;
	pageSize: number;
	searchTerm: string;
	showTags: boolean;
	tagFilters: TagFilters;
}

export default class CommonSearchStore implements ICommonSearchStore {
	@observable public draftsOnly = false;
	@observable public pageSize = 10;
	@observable public searchTerm = '';
	@observable public showTags = false;
	public readonly tagFilters: TagFilters;

	public constructor(values: GlobalValues, tagRepo: TagRepository) {
		makeObservable(this);

		this.tagFilters = new TagFilters(values, tagRepo);
	}
}
