import _ from 'lodash';
import { action, computed, makeObservable, observable } from 'mobx';

import TagApiContract from '../../DataContracts/Tag/TagApiContract';
import TagBaseContract from '../../DataContracts/Tag/TagBaseContract';
import EntryType from '../../Models/EntryType';
import EnglishTranslatedStringEditStore from '../Globalization/EnglishTranslatedStringEditStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class TagEditStore {
	// Bitmask for all possible entry types (all bits 1)
	public static readonly allEntryTypes = 1073741823;

	@observable public categoryName: string;
	@observable public defaultNameLanguage: string;
	public readonly description: EnglishTranslatedStringEditStore;
	public readonly id: number;
	public readonly names: NamesEditStore;
	@observable public parent: TagBaseContract;
	@observable public relatedTags: TagBaseContract[];
	@observable public submitting = false;
	@observable public targets: EntryType;
	@observable public validationExpanded = false;
	public readonly webLinks: WebLinksEditStore;

	public constructor(contract: TagApiContract) {
		makeObservable(this);

		this.categoryName = contract.categoryName;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = new EnglishTranslatedStringEditStore(
			contract.translatedDescription,
		);
		this.id = contract.id;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.parent = contract.parent;
		this.relatedTags = contract.relatedTags;
		this.targets = contract.targets;
		this.webLinks = new WebLinksEditStore(contract.webLinks);

		// TODO
	}

	@computed public get parentName(): string | undefined {
		return this.parent?.name ?? undefined;
	}

	@computed public get validationError_needDescription(): boolean {
		return !this.description.original && _.isEmpty(this.webLinks.items);
	}

	@computed public get hasValidationErrors(): boolean {
		return this.validationError_needDescription;
	}

	@action public addRelatedTag = (tag: TagBaseContract): number => {
		return this.relatedTags.push(tag);
	};

	public denySelf = (tag: TagBaseContract): boolean => {
		return tag && tag.id !== this.id;
	};

	public allowRelatedTag = (tag: TagBaseContract): boolean => {
		return (
			this.denySelf(tag) && _.every(this.relatedTags, (t) => t.id !== tag.id)
		);
	};

	@action public submit = (): boolean => {
		this.submitting = true;
		return true;
	};
}
