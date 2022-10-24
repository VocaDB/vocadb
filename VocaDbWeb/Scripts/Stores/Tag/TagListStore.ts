import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { map } from 'lodash-es';
import { action, computed, makeObservable, observable } from 'mobx';

export class TagListStore {
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
			: this.tagUsages.take(TagListStore.maxDisplayedTags);
	}

	@computed public get tagUsagesByCategories(): {
		categoryName: string;
		tagUsages: TagUsageForApiContract[];
	}[] {
		const tags = map(
			this.tagUsages
				.orderBy((tagUsage) => tagUsage.tag.categoryName)
				.groupBy((tagUsage) => tagUsage.tag.categoryName),
			(tagUsages: TagUsageForApiContract[], categoryName: string) => ({
				categoryName,
				tagUsages,
			}),
		);

		const genres = tags.filter((c) => c.categoryName === 'Genres');
		const empty = tags.filter((c) => c.categoryName === '');

		return genres
			.concat(
				tags.filter(
					(c) => c.categoryName !== 'Genres' && c.categoryName !== '',
				),
			)
			.concat(empty);
	}

	public getTagUrl = (tag: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
	};

	@action public updateTagUsages = (usages: TagUsageForApiContract[]): void => {
		this.tagUsages = usages
			.sortBy((u) => u.tag.name.toLowerCase())
			.sortBy((u) => -u.count);
	};
}
