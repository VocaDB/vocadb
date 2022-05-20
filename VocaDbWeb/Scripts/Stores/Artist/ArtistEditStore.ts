import _ from 'lodash';
import { computed, makeObservable, observable } from 'mobx';
import moment from 'moment';

import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistForArtistContract from '../../DataContracts/Artist/ArtistForArtistContract';
import ArtistForEditContract from '../../DataContracts/Artist/ArtistForEditContract';
import ArtistHelper from '../../Helpers/ArtistHelper';
import ArtistType from '../../Models/Artists/ArtistType';
import ArtistRepository from '../../Repositories/ArtistRepository';
import GlobalValues from '../../Shared/GlobalValues';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
import EntryPictureFileListEditStore from '../EntryPictureFileListEditStore';
import EnglishTranslatedStringEditStore from '../Globalization/EnglishTranslatedStringEditStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export class ArtistForArtistEditStore {
	@observable public linkType: string;
	public readonly parent: ArtistContract;

	public constructor(link: ArtistForArtistContract) {
		makeObservable(this);

		this.linkType = link.linkType;
		this.parent = link.parent;
	}

	public toContract = (): ArtistForArtistContract => {
		return {
			linkType: this.linkType,
			parent: this.parent,
		};
	};
}

export default class ArtistEditStore {
	@observable public artistType: ArtistType;
	@observable public associatedArtists: ArtistForArtistEditStore[];
	public readonly baseVoicebank: BasicEntryLinkStore<ArtistContract>;
	@observable public defaultNameLanguage: string;
	public readonly description: EnglishTranslatedStringEditStore;
	@observable public groups: ArtistForArtistContract[];
	public readonly id: number;
	public readonly illustrator: BasicEntryLinkStore<ArtistContract>;
	public readonly names: NamesEditStore;
	public readonly newAssociatedArtist: BasicEntryLinkStore<ArtistContract>;
	@observable public newAssociatedArtistType?: string;
	public readonly pictures: EntryPictureFileListEditStore;
	@observable public releaseDate?: Date;
	@observable public status: string;
	@observable public submittedJson = '';
	@observable public submitting = false;
	@observable public updateNotes = '';
	@observable public validationExpanded = false;
	public readonly voiceProvider: BasicEntryLinkStore<ArtistContract>;
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		values: GlobalValues,
		artistRepo: ArtistRepository,
		data: ArtistForEditContract,
	) {
		makeObservable(this);

		this.artistType = data.artistType;
		this.associatedArtists = _.map(
			data.associatedArtists,
			(a) => new ArtistForArtistEditStore(a),
		);
		this.baseVoicebank = new BasicEntryLinkStore(
			// TODO: data.baseVoicebank,
			(entryId) =>
				artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.description = new EnglishTranslatedStringEditStore(data.description);
		this.defaultNameLanguage = data.defaultNameLanguage;
		this.groups = data.groups;
		this.id = data.id;
		this.illustrator = new BasicEntryLinkStore(
			// TODO: data.illustrator,
			(entryId) =>
				artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.names = NamesEditStore.fromContracts(data.names);
		this.newAssociatedArtist = new BasicEntryLinkStore<ArtistContract>(
			// TODO: undefined,
			(entryId) =>
				artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.pictures = new EntryPictureFileListEditStore(data.pictures);
		this.releaseDate = data.releaseDate
			? moment(data.releaseDate).toDate()
			: undefined;
		this.status = data.status;
		this.voiceProvider = new BasicEntryLinkStore(
			// TODO: data.voiceProvider,
			(entryId) =>
				artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.webLinks = new WebLinksEditStore(data.webLinks, webLinkCategories);

		// TODO
	}

	private canHaveBaseVoicebank(at: ArtistType): boolean {
		return (
			(ArtistHelper.isVocalistType(at) || at === ArtistType.OtherIndividual) &&
			at !== ArtistType.Vocalist
		);
	}

	@computed public get allowBaseVoicebank(): boolean {
		return this.canHaveBaseVoicebank(this.artistType);
	}

	@computed public get canHaveCircles(): boolean {
		return this.artistType !== ArtistType.Label;
	}

	// Can have related artists (associatedArtists) such as voice provider and illustrator
	@computed public get canHaveRelatedArtists(): boolean {
		return ArtistHelper.canHaveChildVoicebanks(this.artistType);
	}

	@computed public get canHaveReleaseDate(): boolean {
		const vocaloidTypes = [
			ArtistType.Vocaloid,
			ArtistType.UTAU,
			ArtistType.CeVIO,
			ArtistType.OtherVoiceSynthesizer,
			ArtistType.SynthesizerV,
		];
		return _.includes(vocaloidTypes, this.artistType);
	}

	@computed public get validationError_needReferences(): boolean {
		return (
			(this.description.original === undefined ||
				this.description.original.length === 0) /* TODO */ &&
			this.webLinks.items.length === 0
		);
	}

	@computed public get validationError_needType(): boolean {
		return this.artistType === ArtistType.Unknown;
	}

	@computed public get validationError_unnecessaryPName(): boolean {
		const allNames = this.names.getAllNames();
		return _.some(allNames, (n) =>
			_.some(
				allNames,
				(n2) =>
					n !== n2 &&
					(n.value === n2.value + 'P' ||
						n.value === n2.value + '-P' ||
						n.value === n2.value + 'p' ||
						n.value === n2.value + '-p'),
			),
		);
	}

	@computed public get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName();
	}

	@computed public get hasValidationErrors(): boolean {
		return (
			this.validationError_needReferences ||
			this.validationError_needType ||
			this.validationError_unspecifiedNames ||
			this.validationError_unnecessaryPName
		);
	}
}
