import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { TagFilter } from '@/Stores/Search/TagFilter';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

// Manages tag filters for search
export class TagFilters {
	@observable childTags = false;
	@observable tags: TagFilter[] = [];

	constructor(
		private readonly values: GlobalValues,
		private readonly tagRepo: TagRepository,
		tags?: TagFilter[],
	) {
		makeObservable(this);

		this.tags = tags || [];
	}

	@computed get tagIds(): number[] {
		return this.tags.map((t) => t.id);
	}

	// Fired when any of the tag filters is changed
	@computed get filters(): any {
		return {
			tagIds: this.tagIds,
			childTags: this.childTags,
		};
	}

	@action addTag = (tag: TagBaseContract): number =>
		this.tags.push(TagFilter.fromContract(tag));

	@action addTags = (selectedTagIds: number[]): void => {
		if (!selectedTagIds) return;

		const filters = selectedTagIds.map((a) => new TagFilter(a));
		this.tags.push(...filters);

		if (!this.tagRepo) return;

		for (const newTag of filters) {
			const selectedTagId = newTag.id;

			this.tagRepo
				.getById({
					id: selectedTagId,
					lang: this.values.languagePreference,
				})
				.then((tag) => {
					runInAction(() => {
						newTag.name = tag.name;
						newTag.urlSlug = tag.urlSlug;
					});
				});
		}
	};

	@action removeTag = (tag: TagFilter): void => {
		pull(this.tags, tag);
	};
}
