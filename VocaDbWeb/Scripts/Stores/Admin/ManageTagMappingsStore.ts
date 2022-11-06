import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagMappingContract } from '@/DataContracts/Tag/TagMappingContract';
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
	reaction,
	runInAction,
} from 'mobx';

class EditTagMappingStore {
	@observable isDeleted = false;
	readonly isNew: boolean;
	readonly sourceTag: string;
	readonly tag: TagBaseContract;

	constructor(mapping: TagMappingContract, isNew: boolean = false) {
		makeObservable(this);

		this.sourceTag = mapping.sourceTag;
		this.tag = mapping.tag;
		this.isNew = isNew;
	}

	@action deleteMapping = (): void => {
		this.isDeleted = true;
	};
}

export class ManageTagMappingsStore {
	@observable filter = '';
	@observable mappings: EditTagMappingStore[] = [];
	readonly paging = new ServerSidePagingStore(50);
	@observable newSourceName = '';
	readonly newTargetTag: BasicEntryLinkStore<TagBaseContract>;
	@observable submitting = false;

	constructor(private readonly tagRepo: TagRepository) {
		makeObservable(this);

		this.newTargetTag = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId }),
		);

		reaction(
			() => this.filter,
			() => {
				this.paging.totalItems = this.filteredMappings.length;
				this.paging.goToFirstPage();
			},
		);

		this.loadMappings();
	}

	@computed get filteredMappings(): EditTagMappingStore[] {
		const filter = this.filter.toLowerCase();
		if (!filter) return this.mappings;
		return this.mappings.filter(
			(mapping) =>
				mapping.sourceTag.toLowerCase().includes(filter) ||
				mapping.tag.name.toLowerCase().includes(filter),
		);
	}

	@computed get activeMappings(): EditTagMappingStore[] {
		return this.mappings.filter((m) => !m.isDeleted);
	}

	@computed get sortedMappings(): EditTagMappingStore[] {
		return this.filteredMappings.sortBy((m) => m.tag.name.toLowerCase());
	}

	get sortedMappingsPage(): EditTagMappingStore[] {
		return this.sortedMappings.slice(
			this.paging.firstItem,
			this.paging.firstItem + this.paging.pageSize,
		);
	}

	@action addMapping = (): void => {
		if (!this.newSourceName || this.newTargetTag.isEmpty) return;

		this.mappings.push(
			new EditTagMappingStore({
				tag: this.newTargetTag.entry!,
				sourceTag: this.newSourceName,
			}),
		);
		this.newSourceName = '';
		this.newTargetTag.clear();
	};

	@action deleteMapping = (mapping: EditTagMappingStore): void => {
		mapping.isDeleted = true;
	};

	getSourceTagUrl = (tag: EditTagMappingStore): string => {
		return `http://www.nicovideo.jp/tag/${encodeURIComponent(tag.sourceTag)}`;
	};

	getTagUrl = (tag: EditTagMappingStore): string => {
		return functions.mapAbsoluteUrl(
			EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug),
		);
	};

	loadMappings = async (): Promise<void> => {
		const result = await this.tagRepo.getMappings({
			paging: {
				start: 0,
				maxEntries: 10000,
				getTotalCount: false,
			},
		});

		runInAction(() => {
			this.mappings = result.items.map((t) => new EditTagMappingStore(t));
			this.paging.totalItems = this.filteredMappings.length;
			this.paging.goToFirstPage();
		});
	};

	@action save = async (): Promise<void> => {
		try {
			this.submitting = true;

			const mappings = this.activeMappings;
			await this.tagRepo.saveMappings({ mappings: mappings });
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
