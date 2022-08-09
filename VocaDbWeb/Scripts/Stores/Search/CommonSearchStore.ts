import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { makeObservable, observable } from 'mobx';

export interface ICommonSearchStore {
	draftsOnly: boolean;
	pageSize: number;
	searchTerm: string;
	showTags: boolean;
	tagFilters: TagFilters;
}

export class CommonSearchStore implements ICommonSearchStore {
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
