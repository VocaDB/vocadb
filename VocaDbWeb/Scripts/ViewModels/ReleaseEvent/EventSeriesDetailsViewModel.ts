import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryType from '@Models/EntryType';
import UserRepository from '@Repositories/UserRepository';

import TagListViewModel from '../Tag/TagListViewModel';
import TagsEditViewModel from '../Tag/TagsEditViewModel';

export default class EventSeriesDetailsViewModel {
  constructor(
    private readonly userRepo: UserRepository,
    private readonly seriesId: number,
    tagUsages: TagUsageForApiContract[],
  ) {
    this.tagsEditViewModel = new TagsEditViewModel(
      {
        getTagSelections: (callback): Promise<void> =>
          userRepo.getEventSeriesTagSelections(this.seriesId).then(callback),
        saveTagSelections: (tags): void =>
          userRepo.updateEventSeriesTags(
            this.seriesId,
            tags,
            this.tagUsages.updateTagUsages,
          ),
      },
      EntryType.ReleaseEvent /* Event series use event tags for now */,
    );

    this.tagUsages = new TagListViewModel(tagUsages);
  }

  public tagsEditViewModel: TagsEditViewModel;

  public tagUsages: TagListViewModel;
}
