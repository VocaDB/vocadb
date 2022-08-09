import { ResourcesContract } from '@/DataContracts/ResourcesContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { ResourceRepository } from '@/Repositories/ResourceRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { PagedItemsViewModel } from '@/ViewModels/PagedItemsViewModel';
import { TagFilter } from '@/ViewModels/Search/TagFilter';
import { TagFilters } from '@/ViewModels/Search/TagFilters';
import ko from 'knockout';
import moment from 'moment';

enum SongListSortRule {
	Name,
	Date,
	CreateDate,
}

export class SongListsBaseViewModel extends PagedItemsViewModel<SongListContract> {
	public constructor(
		values: GlobalValues,
		resourceRepo: ResourceRepository,
		tagRepo: TagRepository,
		tagIds: number[],
		public showEventDateSort: boolean,
	) {
		super();

		if (!this.showEventDateSort)
			this.sort(SongListSortRule[SongListSortRule.Name]);

		this.tagFilters = new TagFilters(values, tagRepo);

		if (tagIds) this.tagFilters.addTags(tagIds);

		this.query.subscribe(this.clear);
		this.showTags.subscribe(this.clear);
		this.sort.subscribe(this.clear);
		this.tagFilters.tags.subscribe(this.clear);

		resourceRepo
			.getList({
				cultureCode: values.uiCulture,
				setNames: ['songListSortRuleNames'],
			})
			.then((resources) => {
				this.resources(resources);
				this.clear();
			});
	}

	public isFirstForYear = (
		current: SongListContract,
		index: number,
	): boolean => {
		if (this.sort() !== SongListSortRule[SongListSortRule.Date]) return false;

		if (!current.eventDate) return false;

		if (index === 0) return true;

		var prev = this.items()[index - 1];

		if (!prev.eventDate) return false;

		var currentYear = moment(current.eventDate).year();
		var prevYear = moment(prev.eventDate).year();

		return currentYear !== prevYear;
	};

	public query = ko
		.observable('')
		.extend({ rateLimit: { timeout: 300, method: 'notifyWhenChangesStop' } });
	public resources = ko.observable<ResourcesContract>();

	public selectTag = (tag: TagBaseContract): void => {
		this.tagFilters.tags([TagFilter.fromContract(tag)]);
	};

	public showTags = ko.observable(false);
	public sort = ko.observable(SongListSortRule[SongListSortRule.Date]);
	public sortName = ko.computed(() =>
		this.resources() != null
			? this.resources()!.songListSortRuleNames![this.sort()]
			: '',
	);
	public tagFilters: TagFilters;

	public fields = ko.computed(() => {
		return 'MainPicture' + (this.showTags() ? ',Tags' : '');
	});
}
