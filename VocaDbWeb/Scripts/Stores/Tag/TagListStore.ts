import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import _ from 'lodash';
import { action, computed, makeObservable, observable } from 'mobx';

export default class TagListStore {
	private static maxDisplayedTags = 4;

	@observable public expanded = false;
	@observable public tagUsages: TagUsageForApiContract[];

	public constructor(tagUsages: TagUsageForApiContract[]) {
		makeObservable(this);

		this.tagUsages = [];
		this.updateTagUsages(tagUsages);

		if (tagUsages.length <= TagListStore.maxDisplayedTags + 1)
			this.expanded = true;
	}

	@computed public get displayedTagUsages(): TagUsageForApiContract[] {
		return this.expanded
			? this.tagUsages
			: _.take(this.tagUsages, TagListStore.maxDisplayedTags);
	}

	@computed public get tagUsagesByCategories(): {
		categoryName: string;
		tagUsages: TagUsageForApiContract[];
	}[] {
		const tags = _.chain(this.tagUsages)
			.orderBy((tagUsage) => tagUsage.tag.categoryName)
			.groupBy((tagUsage) => tagUsage.tag.categoryName)
			.map((tagUsages: TagUsageForApiContract[], categoryName: string) => ({
				categoryName,
				tagUsages,
			}));

		const genres = tags.filter((c) => c.categoryName === 'Genres').value();
		const empty = tags.filter((c) => c.categoryName === '').value();

		return _.chain(genres)
			.concat(
				tags
					.filter((c) => c.categoryName !== 'Genres' && c.categoryName !== '')
					.value(),
			)
			.concat(empty)
			.value();
	}

	public getTagUrl = (tag: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
	};

	@action public updateTagUsages = (usages: TagUsageForApiContract[]): void => {
		this.tagUsages = _.chain(usages)
			.sortBy((u) => u.tag.name.toLowerCase())
			.sortBy((u) => -u.count)
			.value();
	};
}
