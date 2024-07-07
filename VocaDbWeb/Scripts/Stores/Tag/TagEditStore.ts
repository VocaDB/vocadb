import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagForEditContract } from '@/DataContracts/Tag/TagForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { TagTargetTypes } from '@/Models/Tags/TagTargetTypes';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
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
	readonly deleteStore: DeleteEntryStore;
	readonly description: EnglishTranslatedStringEditStore;
	@observable errors?: Record<string, string[]>;
	@observable hideFromSuggestions: boolean;
	readonly names: NamesEditStore;
	@observable parent: BasicEntryLinkStore<TagBaseContract>;
	@observable relatedTags: TagBaseContract[];
	@observable status: EntryStatus;
	@observable submitting = false;
	@observable targets: TagTargetTypes;
	@observable newTargets: string[]
	readonly trashStore: DeleteEntryStore;
	@observable updateNotes = '';
	readonly webLinks: WebLinksEditStore;

	constructor(
		antiforgeryRepo: AntiforgeryRepository,
		private readonly tagRepo: TagRepository,
		readonly contract: TagForEditContract,
		readonly values: GlobalValues
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				tagRepo.delete(requestToken, {
					id: contract.id,
					notes: notes,
					hardDelete: false,
				}),
		);

		this.trashStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				tagRepo.delete(requestToken, {
					id: contract.id,
					notes: notes,
					hardDelete: true,
				}),
		);

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
		this.newTargets = contract.newTargets;
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
					newTargets: this.newTargets
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

	hasTagTarget = (target: string): boolean => {
		return this.newTargets.includes(target) || this.newTargets.includes(target.split(':')[0])
	}

	@action toggleTarget = (target: string): void => {
		console.log(target)
		const tagTargetTypes = {
			'song': this.values.songTypes,
			'artist': this.values.artistTypes,
			'album': this.values.albumTypes,
			'releaseevent': this.values.eventTypes
		}
		Object.keys(tagTargetTypes).forEach((key) => {
			// @ts-ignore
			tagTargetTypes[key] = tagTargetTypes[key].map(t => t.toLowerCase())
		})

		const [tagType, _] = target.split(':')
		const index = this.newTargets.indexOf(target)
		const tagTypeIndex = this.newTargets.indexOf(tagType)

		if (index === -1 && tagTypeIndex === -1) {
			this.newTargets.push(target)
		} else {
			if (index !== -1) {
				this.newTargets.splice(index, 1)
			}
			if (tagTypeIndex !== -1) {
				this.newTargets.splice(tagTypeIndex, 1)
				this.newTargets.push(...tagTargetTypes[tagType as keyof typeof tagTargetTypes].map(t => [tagType, t].join(":")).filter(t => t !== target))
			}
		}

		// Reduce tag targets
		Object.entries(tagTargetTypes).forEach(([type, subTypes]) => {
			// @ts-ignore
			const includesAllSubtypes = subTypes.filter(t => this.newTargets.includes(`${type}:${t}`)).length === subTypes.length
			if (includesAllSubtypes) {
				this.newTargets = this.newTargets.filter(t => !t.startsWith(type + ':'))
				this.newTargets.push(type)
			}
		})
		console.log(this.newTargets)
	} 

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
