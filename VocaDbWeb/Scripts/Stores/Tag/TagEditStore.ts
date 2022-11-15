import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagForEditContract } from '@/DataContracts/Tag/TagForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { TagTargetTypes } from '@/Models/Tags/TagTargetTypes';
import { TagRepository } from '@/Repositories/TagRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { UrlMapper } from '@/Shared/UrlMapper';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import $ from 'jquery';
import { isEmpty, pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class TagEditStore {
	// Bitmask for all possible entry types (all bits 1)
	static readonly allEntryTypes = 1073741823;

	@observable defaultNameLanguage: string;
	readonly deleteStore = new DeleteEntryStore(
		async (notes) =>
			await $.ajax(
				this.urlMapper.mapRelative(
					`api/tags/${
						this.contract.id
					}?hardDelete=false&notes=${encodeURIComponent(notes)}`,
				),
				{
					type: 'DELETE',
					success: () => {
						window.location.href = EntryUrlMapper.details_tag(this.contract.id);
					},
				},
			),
	);
	readonly description: EnglishTranslatedStringEditStore;
	@observable errors?: Record<string, string[]>;
	@observable hideFromSuggestions: boolean;
	readonly names: NamesEditStore;
	@observable parent: BasicEntryLinkStore<TagBaseContract>;
	@observable relatedTags: TagBaseContract[];
	@observable status: EntryStatus;
	@observable submitting = false;
	@observable targets: TagTargetTypes;
	readonly trashStore = new DeleteEntryStore(
		async (notes) =>
			await $.ajax(
				this.urlMapper.mapRelative(
					`api/tags/${
						this.contract.id
					}?hardDelete=true&notes=${encodeURIComponent(notes)}`,
				),
				{
					type: 'DELETE',
					success: () => {
						window.location.href = this.urlMapper.mapRelative('/Tag');
					},
				},
			),
	);
	@observable updateNotes = '';
	readonly webLinks: WebLinksEditStore;

	constructor(
		private readonly tagRepo: TagRepository,
		private readonly urlMapper: UrlMapper,
		readonly contract: TagForEditContract,
	) {
		makeObservable(this);

		this.parent = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId }),
		);

		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = new EnglishTranslatedStringEditStore(
			contract.description,
		);
		this.hideFromSuggestions = contract.hideFromSuggestions;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.parent.id = contract.parent?.id;
		this.relatedTags = contract.relatedTags;
		this.status = contract.status;
		this.targets = contract.targets;
		this.webLinks = new WebLinksEditStore(contract.webLinks);
	}

	@computed get parentName(): string | undefined {
		return this.parent?.name ?? undefined;
	}

	@computed get validationError_needDescription(): boolean {
		return !this.description.original && isEmpty(this.webLinks.items);
	}

	@computed get hasValidationErrors(): boolean {
		return this.validationError_needDescription;
	}

	@action addRelatedTag = (tag: TagBaseContract): number => {
		return this.relatedTags.push(tag);
	};

	@action removeRelatedTag = (tag: TagBaseContract): void => {
		pull(this.relatedTags, tag);
	};

	denySelf = (tag: TagBaseContract): boolean => {
		return tag && tag.id !== this.contract.id;
	};

	allowRelatedTag = (tag: TagBaseContract): boolean => {
		return this.denySelf(tag) && this.relatedTags.every((t) => t.id !== tag.id);
	};

	@action submit = async (
		requestToken: string,
		categoryName: string /* HACK */,
		thumbPicUpload: File | undefined,
	): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.tagRepo.edit(
				requestToken,
				{
					canDelete: false,
					categoryName: categoryName,
					defaultNameLanguage: this.defaultNameLanguage,
					deleted: false,
					description: this.description.toContract(),
					hideFromSuggestions: this.hideFromSuggestions,
					id: this.contract.id,
					name: '',
					names: this.names.toContracts(),
					parent: this.parent.entry,
					relatedTags: this.relatedTags,
					status: this.status,
					targets: this.targets,
					updateNotes: this.updateNotes,
					webLinks: this.webLinks.items,
				},
				thumbPicUpload,
			);

			return id;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};

	private hasFlag = (t: TagTargetTypes): boolean => (this.targets & t) === t;

	hasTargetType = (target: TagTargetTypes): boolean => {
		return this.hasFlag(target);
	};

	setTargetType = (target: TagTargetTypes, flag: boolean): void => {
		const hasFlag = (t: TagTargetTypes): boolean => (this.targets & t) === t;
		const checkFlags = (): void => {
			const types = [
				TagTargetTypes.Album,
				TagTargetTypes.Artist,
				TagTargetTypes.Event,
				TagTargetTypes.Song,
			];
			if (this.targets === types.sum()) {
				this.targets = TagEditStore.allEntryTypes;
			} else {
				this.targets = types.filter((t) => hasFlag(t)).sum();
			}
		};
		const addFlag = (): void => {
			this.targets = this.targets | target;
			checkFlags();
		};
		const removeFlag = (): void => {
			if (hasFlag(target)) {
				this.targets = this.targets - target;
				checkFlags();
			}
		};
		return flag ? addFlag() : removeFlag();
	};
}
