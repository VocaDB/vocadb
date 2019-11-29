
//namespace vdb.viewModels.releaseEvents {

	import cls = vdb.models;

	export class EventSeriesDetailsViewModel {

		constructor(
			private readonly userRepo: rep.UserRepository,
			private readonly seriesId: number,
			tagUsages: dc.tags.TagUsageForApiContract[]
		) {

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepo.getEventSeriesTagSelections(this.seriesId, callback),
				saveTagSelections: tags => userRepo.updateEventSeriesTags(this.seriesId, tags, this.tagUsages.updateTagUsages)
			}, cls.EntryType.ReleaseEvent /* Event series use event tags for now */);

			this.tagUsages = new tags.TagListViewModel(tagUsages);

		}

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

	}

//}