
import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import ServerSidePagingViewModel from '../ServerSidePagingViewModel';
import TagBaseContract from '../../DataContracts/Tag/TagBaseContract';
import TagMappingContract from '../../DataContracts/Tag/TagMappingContract';
import TagRepository from '../../Repositories/TagRepository';
import ui from '../../Shared/MessagesTyped';

//namespace vdb.viewModels.admin {

	export class ManageTagMappingsViewModel {

		constructor(
			private readonly tagRepo: TagRepository) {
			this.filter.subscribe(() => {
				this.paging.totalItems(this.filteredMappings().length);
				this.paging.goToFirstPage();
			})
			this.loadMappings();
		}

		public addMapping = () => {

			if (!this.newSourceName || this.newTargetTag.isEmpty())
				return;

			if (_.some(this.mappings(), m => m.tag.id === this.newTargetTag.id() && m.sourceTag.toLowerCase() === this.newSourceName().toLowerCase())) {
				ui.showErrorMessage("Mapping already exists for source tag " + this.newSourceName());
				return;
			}

			this.mappings.push(new EditTagMappingViewModel({ tag: this.newTargetTag.entry(), sourceTag: this.newSourceName() }, true));
			this.newSourceName("");
			this.newTargetTag.clear();

		}

		public deleteMapping = (mapping: EditTagMappingViewModel) => {
			mapping.isDeleted(true);
		}

		public filter = ko.observable("");

		public getSourceTagUrl = (tag: EditTagMappingViewModel) => {
			return "http://www.nicovideo.jp/tag/" + encodeURIComponent(tag.sourceTag);
		}

		public getTagUrl = (tag: EditTagMappingViewModel) => {
			return vdb.functions.mapFullUrl(EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug));
		}

		private loadMappings = async () => {

			const result = await this.tagRepo.getMappings({ start: 0, maxEntries: 1000, getTotalCount: false });
			this.mappings(_.map(result.items, t => new EditTagMappingViewModel(t)));
			this.paging.totalItems(this.filteredMappings().length);
			this.paging.goToFirstPage();

		}

		public mappings = ko.observableArray<EditTagMappingViewModel>();

		public filteredMappings = ko.computed(() => {
			const filter = this.filter().toLowerCase();
			if (!filter)
				return this.mappings();
			return _.filter(this.mappings(), mapping => _.includes(mapping.sourceTag.toLowerCase(), filter) || _.includes(mapping.tag.name.toLowerCase(), filter));
		});

		public paging = new ServerSidePagingViewModel(50);

		public activeMappings = ko.computed(() => _.filter(this.mappings(), m => !m.isDeleted()));

		public newSourceName = ko.observable("");
		public newTargetTag = new BasicEntryLinkViewModel<TagBaseContract>();

		public save = async () => {

			const mappings = this.activeMappings();
			await this.tagRepo.saveMappings(mappings);
			ui.showSuccessMessage("Saved");
			await this.loadMappings();

		}

		public sortedMappings = ko.computed(() => _.sortBy(this.filteredMappings(), m => m.tag.name.toLowerCase()));

		public sortedMappingsPage = ko.computed(() => {
			return this.sortedMappings().slice(this.paging.firstItem(), this.paging.firstItem() + this.paging.pageSize());
		});

	}

	export class EditTagMappingViewModel {

		constructor(mapping: TagMappingContract, isNew: boolean = false) {
			this.sourceTag = mapping.sourceTag;
			this.tag = mapping.tag;
			this.isNew = isNew;
		}

		isDeleted = ko.observable(false);
		isNew: boolean;
		sourceTag: string;
		tag: TagBaseContract;

		public deleteMapping = () => this.isDeleted(true);

	}

//}