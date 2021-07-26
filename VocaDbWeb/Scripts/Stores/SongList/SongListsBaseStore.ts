import SongListContract from '@DataContracts/Song/SongListContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagRepository from '@Repositories/TagRepository';
import GlobalValues from '@Shared/GlobalValues';
import PagedItemsStore from '@Stores/PagedItemsStore';
import TagFilter from '@Stores/Search/TagFilter';
import TagFilters from '@Stores/Search/TagFilters';
import debounceEffect from '@Stores/debounceEffect';
import { action, computed, makeObservable, observable, reaction } from 'mobx';
import moment from 'moment';

export enum SongListSortRule {
	Name = 'Name',
	Date = 'Date',
	CreateDate = 'CreateDate',
}

export default abstract class SongListsBaseStore extends PagedItemsStore<SongListContract> {
	@observable public query = '';
	@observable public showTags = false;
	@observable public sort = SongListSortRule.Date;
	public readonly tagFilters: TagFilters;

	protected constructor(
		values: GlobalValues,
		tagRepo: TagRepository,
		tagIds: number[],
		public readonly showEventDateSort: boolean,
	) {
		super();

		makeObservable(this);

		if (!showEventDateSort) this.sort = SongListSortRule.Name;

		this.tagFilters = new TagFilters(values, tagRepo);

		if (tagIds) this.tagFilters.addTags(tagIds);

		reaction(() => this.query, debounceEffect(this.clear, 300));
		reaction(() => this.showTags, this.clear);
		reaction(() => this.sort, this.clear);
		reaction(() => this.tagFilters.tags.map((tag) => tag.id), this.clear);
	}

	@computed public get fields(): string {
		return 'MainPicture' + (this.showTags ? ',Tags' : '');
	}

	public isFirstForYear = (
		current: SongListContract,
		index: number,
	): boolean => {
		if (this.sort !== SongListSortRule.Date) return false;

		if (!current.eventDate) return false;

		if (index === 0) return true;

		const prev = this.items[index - 1];

		if (!prev.eventDate) return false;

		const currentYear = moment(current.eventDate).year();
		const prevYear = moment(prev.eventDate).year();

		return currentYear !== prevYear;
	};

	@action public selectTag = (tag: TagBaseContract): void => {
		this.tagFilters.tags = [TagFilter.fromContract(tag)];
	};
}
