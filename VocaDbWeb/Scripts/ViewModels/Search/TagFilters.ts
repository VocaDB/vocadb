import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { TagFilter } from '@/ViewModels/Search/TagFilter';
import ko, { Computed, Observable, ObservableArray } from 'knockout';

// Manages tag filters for search
export class TagFilters {
	public constructor(
		private readonly values: GlobalValues,
		private tagRepo: TagRepository,
		tags: ObservableArray<TagFilter> = null!,
	) {
		this.tags = tags || ko.observableArray<TagFilter>();
		this.tagIds = ko.computed(() => this.tags().map((t) => t.id));
		this.childTags = ko.observable(false);

		this.filters = ko
			.computed(() => {
				this.tags();
				this.childTags();
			})
			.extend({ notify: 'always' });
	}

	public addTag = (tag: TagBaseContract): number =>
		this.tags.push(TagFilter.fromContract(tag));

	public addTags = (selectedTagIds: number[]): void => {
		if (!selectedTagIds) return;

		var filters = selectedTagIds.map((a) => new TagFilter(a));
		ko.utils.arrayPushAll(this.tags, filters);

		if (!this.tagRepo) return;

		for (const newTag of filters) {
			var selectedTagId = newTag.id;

			this.tagRepo
				.getById({
					id: selectedTagId,
					lang: this.values.languagePreference,
				})
				.then((tag) => {
					newTag.name(tag.name);
					newTag.urlSlug(tag.urlSlug!);
				});
		}
	};

	public childTags: Observable<boolean>;

	// Fired when any of the tag filters is changed
	public filters: Computed<void>;

	public selectTag = (tag: TagBaseContract): void => {
		this.tags([TagFilter.fromContract(tag)]);
	};

	public tags: ObservableArray<TagFilter>;
	public tagIds: Computed<number[]>;
}
