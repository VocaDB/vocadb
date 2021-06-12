import EntryTypeAndSubTypeContract from '@DataContracts/EntryTypeAndSubTypeContract';
import EntryTagMappingContract from '@DataContracts/Tag/EntryTagMappingContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import AlbumType from '@Models/Albums/AlbumType';
import ArtistType from '@Models/Artists/ArtistType';
import EntryType from '@Models/EntryType';
import EventCategory from '@Models/Events/EventCategory';
import SongType from '@Models/Songs/SongType';
import TagRepository from '@Repositories/TagRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import functions from '@Shared/GlobalFunctions';
import ui from '@Shared/MessagesTyped';
import ko, { Computed } from 'knockout';
import _ from 'lodash';

import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import ServerSidePagingViewModel from '../ServerSidePagingViewModel';
import { EditTagMappingViewModel } from './ManageTagMappingsViewModel';

export default class ManageEntryTagMappingsViewModel {
	public constructor(private readonly tagRepo: TagRepository) {
		this.loadMappings();
	}

	public addMapping = (): void => {
		if (!this.newEntryType || this.newTargetTag.isEmpty()) return;

		if (
			_.some(
				this.mappings(),
				(m) =>
					m.tag.id === this.newTargetTag.id() &&
					m.entryType.entryType === this.newEntryType() &&
					m.entryType.subType === this.newEntrySubType(),
			)
		) {
			ui.showErrorMessage(
				'Mapping already exists for entry type ' +
					this.newEntryType() +
					', ' +
					this.newEntrySubType(),
			);
			return;
		}

		this.mappings.push(
			new EditEntryTagMappingViewModel(
				{
					tag: this.newTargetTag.entry(),
					entryType: {
						entryType: this.newEntryType(),
						subType: this.newEntrySubType(),
					},
				},
				true,
			),
		);
		this.newEntrySubType('');
		this.newEntryType('');
		this.newTargetTag.clear();
	};

	public deleteMapping = (mapping: EditTagMappingViewModel): void => {
		mapping.isDeleted(true);
	};

	public getTagUrl = (tag: EditTagMappingViewModel): string => {
		return functions.mapAbsoluteUrl(
			EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug),
		);
	};

	private loadMappings = async (): Promise<void> => {
		const result = await this.tagRepo.getEntryTagMappings();
		this.mappings(_.map(result, (t) => new EditEntryTagMappingViewModel(t)));
	};

	public mappings = ko.observableArray<EditEntryTagMappingViewModel>();

	public paging = new ServerSidePagingViewModel(50);

	public activeMappings = ko.computed(() =>
		_.filter(this.mappings(), (m) => !m.isDeleted()),
	);

	private getEnumValues = <TEnum>(
		Enum: any,
		selected?: Array<TEnum>,
	): string[] =>
		Object.keys(Enum).filter(
			(k) =>
				(!selected || _.includes(selected, Enum[k])) &&
				typeof Enum[k as any] === 'number',
		);

	public entryTypes = this.getEnumValues<EntryType>(EntryType, [
		EntryType.Album,
		EntryType.Artist,
		EntryType.Song,
		EntryType.ReleaseEvent,
	]);

	private readonly entrySubTypesByType = [
		{ key: EntryType.Album, values: this.getEnumValues<AlbumType>(AlbumType) },
		{
			key: EntryType.Artist,
			values: this.getEnumValues<ArtistType>(ArtistType),
		},
		{ key: EntryType.Song, values: this.getEnumValues<SongType>(SongType) },
		{
			key: EntryType.ReleaseEvent,
			values: this.getEnumValues<EventCategory>(EventCategory),
		},
	];

	public newEntryType = ko.observable('');
	public newEntrySubType = ko.observable('');
	public newTargetTag = new BasicEntryLinkViewModel<TagBaseContract>();

	public entrySubTypes: Computed<string[]> = ko.computed(
		() =>
			_.find(
				this.entrySubTypesByType,
				(et) => EntryType[et.key] === this.newEntryType(),
			)?.values ?? [],
	);

	public save = async (): Promise<void> => {
		const mappings = this.activeMappings();
		await this.tagRepo.saveEntryMappings(mappings);
		ui.showSuccessMessage('Saved');
		await this.loadMappings();
	};
}

export class EditEntryTagMappingViewModel {
	public constructor(mapping: EntryTagMappingContract, isNew: boolean = false) {
		this.entryType = mapping.entryType;
		this.tag = mapping.tag;
		this.isNew = isNew;
	}

	public isDeleted = ko.observable(false);
	public isNew: boolean;
	public entryType: EntryTypeAndSubTypeContract;
	public tag: TagBaseContract;

	public deleteMapping = (): void => this.isDeleted(true);
}
