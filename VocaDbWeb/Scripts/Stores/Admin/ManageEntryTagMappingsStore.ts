import { EntryTypeAndSubTypeContract } from '@/DataContracts/EntryTypeAndSubTypeContract';
import { EntryTagMappingContract } from '@/DataContracts/Tag/EntryTagMappingContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryType } from '@/Models/EntryType';
import { EventCategory } from '@/Models/Events/EventCategory';
import { SongType } from '@/Models/Songs/SongType';
import { TagRepository } from '@/Repositories/TagRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

class EditEntryTagMappingStore {
	@observable isDeleted = false;
	readonly isNew: boolean;
	readonly entryType: EntryTypeAndSubTypeContract;
	readonly tag: TagBaseContract;

	constructor(mapping: EntryTagMappingContract, isNew: boolean = false) {
		makeObservable(this);

		this.entryType = mapping.entryType;
		this.tag = mapping.tag;
		this.isNew = isNew;
	}

	@action deleteMapping = (): void => {
		this.isDeleted = true;
	};
}

export class ManageEntryTagMappingsStore {
	@observable mappings: EditEntryTagMappingStore[] = [];
	readonly paging = new ServerSidePagingStore(50);
	@observable newEntryType = '';
	@observable newEntrySubType = '';
	readonly newTargetTag: BasicEntryLinkStore<TagBaseContract>;
	@observable submitting = false;

	constructor(private readonly tagRepo: TagRepository) {
		makeObservable(this);

		this.newTargetTag = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId }),
		);

		this.loadMappings();
	}

	@computed get activeMappings(): EditEntryTagMappingStore[] {
		return this.mappings.filter((m) => !m.isDeleted);
	}

	private getEnumValues = <TEnum>(
		Enum: any,
		selected?: Array<TEnum>,
	): string[] =>
		Object.keys(Enum).filter(
			(k) =>
				(!selected || selected.includes(Enum[k])) &&
				typeof Enum[k as any] === 'number',
		);

	entryTypes = this.getEnumValues<EntryType>(EntryType, [
		EntryType.Album,
		EntryType.Artist,
		EntryType.Song,
		EntryType.ReleaseEvent,
	]);

	private readonly entrySubTypesByType = {
		[EntryType.Album]: Object.values(AlbumType),
		[EntryType.Artist]: Object.values(ArtistType),
		[EntryType.Song]: Object.values(SongType),
		[EntryType.ReleaseEvent]: Object.values(EventCategory),
	};

	@computed get entrySubTypes():
		| AlbumType[]
		| ArtistType[]
		| SongType[]
		| EventCategory[] {
		return this.entrySubTypesByType[this.newEntryType] ?? [];
	}

	@action addMapping = (): void => {
		if (!this.newEntryType || this.newTargetTag.isEmpty) return;

		this.mappings.push(
			new EditEntryTagMappingStore(
				{
					tag: this.newTargetTag.entry!,
					entryType: {
						entryType: this.newEntryType,
						subType: this.newEntrySubType,
					},
				},
				true,
			),
		);
		this.newEntrySubType = '';
		this.newEntryType = '';
		this.newTargetTag.clear();
	};

	@action deleteMapping = (mapping: EditEntryTagMappingStore): void => {
		mapping.isDeleted = true;
	};

	getTagUrl = (tag: EditEntryTagMappingStore): string => {
		return functions.mapAbsoluteUrl(
			EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug),
		);
	};

	loadMappings = async (): Promise<void> => {
		const result = await this.tagRepo.getEntryTagMappings({});

		runInAction(() => {
			this.mappings = result.map((t) => new EditEntryTagMappingStore(t));
		});
	};

	@action save = async (): Promise<void> => {
		try {
			this.submitting = true;

			const mappings = this.activeMappings;
			await this.tagRepo.saveEntryMappings({ mappings: mappings });
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
