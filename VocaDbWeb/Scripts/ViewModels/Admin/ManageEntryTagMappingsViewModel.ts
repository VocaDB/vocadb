
namespace vdb.viewModels.admin {

	import dc = dataContracts;
	import cls = models;
	import vm = viewModels;

	export class ManageEntryTagMappingsViewModel {

		constructor(
			private readonly tagRepo: vdb.repositories.TagRepository) {
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
			return vdb.functions.mapAbsoluteUrl(utils.EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug));
		}

		private loadMappings = async () => {

			const result = await this.tagRepo.getEntryTagMappings();
			this.mappings(_.map(result, t => new EditEntryTagMappingViewModel(t)));

		}

		public mappings = ko.observableArray<EditEntryTagMappingViewModel>();

		public paging = new vm.ServerSidePagingViewModel(50);

		public activeMappings = ko.computed(() => _.filter(this.mappings(), m => !m.isDeleted()));

		private getEnumValues = <TEnum>(Enum: any, selected?: Array<TEnum>) => Object.keys(Enum).filter(k => (!selected || _.includes(selected, Enum[k])) && typeof Enum[k as any] === "number");

		public entryTypes = this.getEnumValues<cls.EntryType>(cls.EntryType, [cls.EntryType.Album, cls.EntryType.Artist, cls.EntryType.Song, cls.EntryType.ReleaseEvent]);

		private readonly entrySubTypesByType = [
			{ key: cls.EntryType.Album, values: this.getEnumValues<cls.albums.AlbumType>(cls.albums.AlbumType) },
			{ key: cls.EntryType.Artist, values: this.getEnumValues<cls.artists.ArtistType>(cls.artists.ArtistType) },
			{ key: cls.EntryType.Song, values: this.getEnumValues<cls.songs.SongType>(cls.songs.SongType) },
			{ key: cls.EntryType.ReleaseEvent, values: this.getEnumValues<cls.events.EventCategory>(cls.events.EventCategory) }
		];

		public newEntryType = ko.observable("");
		public newEntrySubType = ko.observable("");
		public newTargetTag = new BasicEntryLinkViewModel<dc.TagBaseContract>();

		public entrySubTypes: KnockoutComputed<string[]> = ko.computed(() => _.find(this.entrySubTypesByType, et => cls.EntryType[et.key] === this.newEntryType())?.values ?? []);

		public save = async () => {

			const mappings = this.activeMappings();
			await this.tagRepo.saveEntryMappings(mappings);
			ui.showSuccessMessage("Saved");
			await this.loadMappings();

		}

	}

	export class EditEntryTagMappingViewModel {

		constructor(mapping: dc.tags.EntryTagMappingContract, isNew: boolean = false) {
			this.entryType = mapping.entryType;
			this.tag = mapping.tag;
			this.isNew = isNew;
		}

		isDeleted = ko.observable(false);
		isNew: boolean;
		entryType: dc.EntryTypeAndSubTypeContract;
		tag: dc.TagBaseContract;

		public deleteMapping = () => this.isDeleted(true);

	}

}