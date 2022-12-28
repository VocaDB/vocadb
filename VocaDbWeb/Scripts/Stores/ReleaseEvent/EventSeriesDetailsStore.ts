import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { TagTargetType } from '@/Models/Tags/TagTargetType';
import { UserRepository } from '@/Repositories/UserRepository';
import { TagListStore } from '@/Stores/Tag/TagListStore';
import { TagsEditStore } from '@/Stores/Tag/TagsEditStore';

export class EventSeriesDetailsStore {
	readonly tagsEditStore: TagsEditStore;
	readonly tagUsages: TagListStore;

	constructor(
		userRepo: UserRepository,
		private readonly seriesId: number,
		tagUsages: TagUsageForApiContract[],
	) {
		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getEventSeriesTagSelections({ seriesId: this.seriesId }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateEventSeriesTags({
							seriesId: this.seriesId,
							tags: tags,
						})
						.then(this.tagUsages.updateTagUsages),
			},
			TagTargetType.Event /* Event series use event tags for now */,
		);

		this.tagUsages = new TagListStore(tagUsages);
	}
}
