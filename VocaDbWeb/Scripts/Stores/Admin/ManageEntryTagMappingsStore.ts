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
	@observable public isDeleted = false;
	public readonly isNew: boolean;
	public readonly entryType: EntryTypeAndSubTypeContract;
	public readonly tag: TagBaseContract;

	public constructor(mapping: EntryTagMappingContract, isNew: boolean = false) {
		makeObservable(this);

		this.entryType = mapping.entryType;
		this.tag = mapping.tag;
		this.isNew = isNew;
	}

	@action public deleteMapping = (): void => {
		this.isDeleted = true;
	};
}

export class ManageEntryTagMappingsStore {
	@observable public mappings: EditEntryTagMappingStore[] = [];
	public readonly paging = new ServerSidePagingStore(50);
	@observable public newEntryType = '';
	@observable public newEntrySubType = '';
	public readonly newTargetTag: BasicEntryLinkStore<TagBaseContract>;
	@observable public submitting = false;

	public constructor(private readonly tagRepo: TagRepository) {
		makeObservable(this);

		this.newTargetTag = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId }),
		);

		this.loadMappings();
	}

	@computed public get activeMappings(): EditEntryTagMappingStore[] {
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

	public entryTypes = this.getEnumValues<EntryType>(EntryType, [
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

	@computed public get entrySubTypes():
		| AlbumType[]
		| ArtistType[]
		| SongType[]
		| EventCategory[] {
		return (
			this.entrySubTypesByType[
				EntryType[this.newEntryType as keyof typeof EntryType]
			] ?? []
		);
	}

	@action public addMapping = (): void => {
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

	@action public deleteMapping = (mapping: EditEntryTagMappingStore): void => {
		mapping.isDeleted = true;
	};

	public getTagUrl = (tag: EditEntryTagMappingStore): string => {
		return functions.mapAbsoluteUrl(
			EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug),
		);
	};

	public loadMappings = async (): Promise<void> => {
		const result = await this.tagRepo.getEntryTagMappings({});
		runInAction(() => {
			this.mappings = result.map((t) => new EditEntryTagMappingStore(t));
		});
	};

	@action public save = async (): Promise<void> => {
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
