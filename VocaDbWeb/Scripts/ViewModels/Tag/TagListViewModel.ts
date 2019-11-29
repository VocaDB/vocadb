
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import TagUsageForApiContract from '../../DataContracts/Tag/TagUsageForApiContract';

//module vdb.viewModels.tags {
	
	export default class TagListViewModel {
		
		private static maxDisplayedTags = 4;

		constructor(tagUsages: TagUsageForApiContract[]) {
			
			this.tagUsages = ko.observableArray([]);
			this.updateTagUsages(tagUsages);

			if (tagUsages.length <= TagListViewModel.maxDisplayedTags + 1)
				this.expanded(true);

			this.displayedTagUsages = ko.computed(() =>
				(this.expanded() ? this.tagUsages() : _.take(this.tagUsages(), TagListViewModel.maxDisplayedTags))
			);

		}

		public displayedTagUsages: KnockoutComputed<TagUsageForApiContract[]>;

		public getTagUrl = (tag: TagUsageForApiContract) => {
			return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
		}

		public expanded = ko.observable(false);

		public tagUsages: KnockoutObservableArray<TagUsageForApiContract>;

		public updateTagUsages = (usages: TagUsageForApiContract[]) => {
			this.tagUsages(_.chain(usages).sortBy(u => u.tag.name.toLowerCase()).sortBy(u => -u.count).value());
		}

	}

//} 