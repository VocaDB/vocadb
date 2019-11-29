
import EntryType from '../../Models/EntryType';
import TagUsageForApiContract from '../../DataContracts/Tag/TagUsageForApiContract';
import TagListViewModel from '../Tag/TagListViewModel';
import TagsEditViewModel from '../Tag/TagsEditViewModel';
import UserRepository from '../../Repositories/UserRepository';

//namespace vdb.viewModels.releaseEvents {

	export class EventSeriesDetailsViewModel {

		constructor(
			private readonly userRepo: UserRepository,
			private readonly seriesId: number,
			tagUsages: TagUsageForApiContract[]
		) {

			this.tagsEditViewModel = new TagsEditViewModel({
				getTagSelections: callback => userRepo.getEventSeriesTagSelections(this.seriesId, callback),
				saveTagSelections: tags => userRepo.updateEventSeriesTags(this.seriesId, tags, this.tagUsages.updateTagUsages)
			}, EntryType.ReleaseEvent /* Event series use event tags for now */);

			this.tagUsages = new TagListViewModel(tagUsages);

		}

		public tagsEditViewModel: TagsEditViewModel;

		public tagUsages: TagListViewModel;

	}

//}