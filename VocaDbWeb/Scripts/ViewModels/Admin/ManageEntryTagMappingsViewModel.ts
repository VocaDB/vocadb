
import AlbumType from "../../Models/Albums/AlbumType";
import ArtistType from "../../Models/Artists/ArtistType";
import BasicEntryLinkViewModel from "../BasicEntryLinkViewModel";
import { EditTagMappingViewModel } from "./ManageTagMappingsViewModel";
import EntryTagMappingContract from "../../DataContracts/Tag/EntryTagMappingContract";
import EntryType from "../../Models/EntryType";
import EntryTypeAndSubTypeContract from "../../DataContracts/EntryTypeAndSubTypeContract";
import EntryUrlMapper from "../../Shared/EntryUrlMapper";
import EventCategory from "../../Models/Events/EventCategory";
import ServerSidePagingViewModel from "../ServerSidePagingViewModel";
import SongType from "../../Models/Songs/SongType";
import TagBaseContract from "../../DataContracts/Tag/TagBaseContract";
import TagRepository from "../../Repositories/TagRepository";
import ui from '../../Shared/MessagesTyped';

//namespace vdb.viewModels.admin {

	export default class ManageEntryTagMappingsViewModel {

		constructor(
			private readonly tagRepo: TagRepository) {
			this.loadMappings();
		}

		public addMapping = () => {

			if (!this.newEntryType || this.newTargetTag.isEmpty())
				return;

			if (_.some(this.mappings(), m => m.tag.id === this.newTargetTag.id()
				&& m.entryType.entryType === this.newEntryType()
				&& m.entryType.subType === this.newEntrySubType())) {
				ui.showErrorMessage("Mapping already exists for entry type " + this.newEntryType() + ", " + this.newEntrySubType());
				return;
			}

			this.mappings.push(new EditEntryTagMappingViewModel({ tag: this.newTargetTag.entry(), entryType: { entryType: this.newEntryType(), subType: this.newEntrySubType() } }, true));
			this.newEntrySubType("");
			this.newEntryType("");
			this.newTargetTag.clear();

		}

		public deleteMapping = (mapping: EditTagMappingViewModel) => {
			mapping.isDeleted(true);
		}

		public getTagUrl = (tag: EditTagMappingViewModel) => {
			return vdb.functions.mapFullUrl(EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug));
		}

		private loadMappings = async () => {

			const result = await this.tagRepo.getEntryTagMappings();
			this.mappings(_.map(result, t => new EditEntryTagMappingViewModel(t)));

		}

		public mappings = ko.observableArray<EditEntryTagMappingViewModel>();

		public paging = new ServerSidePagingViewModel(50);

		public activeMappings = ko.computed(() => _.filter(this.mappings(), m => !m.isDeleted()));

		private getEnumValues = <TEnum>(Enum: any, selected?: Array<TEnum>) => Object.keys(Enum).filter(k => (!selected || _.includes(selected, Enum[k])) && typeof Enum[k as any] === "number");

		public entryTypes = this.getEnumValues<EntryType>(EntryType, [EntryType.Album, EntryType.Artist, EntryType.Song, EntryType.ReleaseEvent]);

		private readonly entrySubTypesByType = [
			{ key: EntryType.Album, values: this.getEnumValues<AlbumType>(AlbumType) },
			{ key: EntryType.Artist, values: this.getEnumValues<ArtistType>(ArtistType) },
			{ key: EntryType.Song, values: this.getEnumValues<SongType>(SongType) },
			{ key: EntryType.ReleaseEvent, values: this.getEnumValues<EventCategory>(EventCategory) }
		];

		public newEntryType = ko.observable("");
		public newEntrySubType = ko.observable("");
		public newTargetTag = new BasicEntryLinkViewModel<TagBaseContract>();

		public entrySubTypes: KnockoutComputed<string[]> = ko.computed(() => _.find(this.entrySubTypesByType, et => EntryType[et.key] === this.newEntryType())?.values ?? []);

		public save = async () => {

			const mappings = this.activeMappings();
			await this.tagRepo.saveEntryMappings(mappings);
			ui.showSuccessMessage("Saved");
			await this.loadMappings();

		}

	}

	export class EditEntryTagMappingViewModel {

		constructor(mapping: EntryTagMappingContract, isNew: boolean = false) {
			this.entryType = mapping.entryType;
			this.tag = mapping.tag;
			this.isNew = isNew;
		}

		isDeleted = ko.observable(false);
		isNew: boolean;
		entryType: EntryTypeAndSubTypeContract;
		tag: TagBaseContract;

		public deleteMapping = () => this.isDeleted(true);

	}

//}