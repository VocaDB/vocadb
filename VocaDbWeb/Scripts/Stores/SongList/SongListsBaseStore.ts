import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { SongListOptionalField } from '@/Repositories/SongListRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { PagedItemsStore } from '@/Stores/PagedItemsStore';
import { TagFilter } from '@/Stores/Search/TagFilter';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { action, computed, makeObservable, observable, reaction } from 'mobx';
import moment from 'moment';

// Corresponds to the SongListSortRule enum in C#.
export enum SongListSortRule {
	Name = 'Name',
	Date = 'Date',
	CreateDate = 'CreateDate',
}

interface SongListsBaseRouteParams {
	filter?: string;
	sort?: SongListSortRule;
	tagId?: number | number[];
}

export abstract class SongListsBaseStore extends PagedItemsStore<SongListContract> {
	@observable query = '';
	@observable showTags = false;
	@observable sort = SongListSortRule.Date;
	readonly tagFilters: TagFilters;

	protected constructor(
		values: GlobalValues,
		tagRepo: TagRepository,
		tagIds: number[],
		readonly showEventDateSort: boolean,
	) {
		super();

		makeObservable(this);

		if (!showEventDateSort) this.sort = SongListSortRule.Name;

		this.tagFilters = new TagFilters(values, tagRepo);

		if (tagIds) this.tagFilters.addTags(tagIds);

		reaction(() => this.showTags, this.clear);
	}

	@computed get tags(): TagFilter[] {
		return this.tagFilters.tags;
	}
	set tags(value: TagFilter[]) {
		this.tagFilters.tags = value;
	}

	@computed get tagIds(): number[] {
		return this.tags.map((t) => t.id);
	}
	set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	@computed get fields(): SongListOptionalField[] {
		return this.showTags
			? [SongListOptionalField.MainPicture, SongListOptionalField.Tags]
			: [SongListOptionalField.MainPicture];
	}

	isFirstForYear = (current: SongListContract, index: number): boolean => {
		if (this.sort !== SongListSortRule.Date) return false;

		if (!current.eventDate) return false;

		if (index === 0) return true;

		const prev = this.items[index - 1];

		if (!prev.eventDate) return false;

		const currentYear = moment(current.eventDate).year();
		const prevYear = moment(prev.eventDate).year();

		return currentYear !== prevYear;
	};

	@action selectTag = (tag: TagBaseContract): void => {
		this.tagFilters.tags = [TagFilter.fromContract(tag)];
	};

	@computed.struct get locationState(): SongListsBaseRouteParams {
		return {
			filter: this.query,
			sort: this.sort,
			tagId: this.tagIds,
		};
	}
	set locationState(value: SongListsBaseRouteParams) {
		this.query = value.filter ?? '';
		this.sort = value.sort || SongListSortRule.Date;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
	}
}
