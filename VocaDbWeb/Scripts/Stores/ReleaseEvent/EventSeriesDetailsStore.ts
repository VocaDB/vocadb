import TagSelectionContract from '@DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryType from '@Models/EntryType';
import UserRepository from '@Repositories/UserRepository';
import TagListStore from '@Stores/Tag/TagListStore';
import TagsEditStore from '@Stores/Tag/TagsEditStore';

export default class EventSeriesDetailsStore {
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagUsages: TagListStore;

	public constructor(
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
			EntryType.ReleaseEvent /* Event series use event tags for now */,
		);

		this.tagUsages = new TagListStore(tagUsages);
	}
}
