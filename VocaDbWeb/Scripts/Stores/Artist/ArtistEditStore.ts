import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForArtistContract } from '@/DataContracts/Artist/ArtistForArtistContract';
import { ArtistForEditContract } from '@/DataContracts/Artist/ArtistForEditContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistLinkType } from '@/Models/Artists/ArtistLinkType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EntryPictureFileListEditStore } from '@/Stores/EntryPictureFileListEditStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import dayjs from 'dayjs';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

import { CultureCodesEditStore } from '../CultureCodesEditStore';

export class ArtistForArtistEditStore {
	@observable linkType: string /* TODO: enum */;
	parent: ArtistContract;

	constructor(link: ArtistForArtistContract) {
		makeObservable(this);

		this.linkType = link.linkType!;
		this.parent = link.parent;
	}

	toContract = (): ArtistForArtistContract => {
		return {
			linkType: this.linkType,
			parent: this.parent,
		};
	};
}

export class ArtistEditStore {
	@observable artistType: ArtistType;
	@observable associatedArtists: ArtistForArtistEditStore[];
	readonly baseVoicebank: BasicEntryLinkStore<ArtistContract>;
	@observable defaultNameLanguage: string;
	readonly deleteStore: DeleteEntryStore;
	readonly description: EnglishTranslatedStringEditStore;
	@observable errors?: Record<string, string[]>;
	@observable groups: ArtistForArtistContract[];
	readonly illustrator: BasicEntryLinkStore<ArtistContract>;
	readonly names: NamesEditStore;
	readonly newAssociatedArtist: BasicEntryLinkStore<ArtistContract>;
	@observable newAssociatedArtistType = ArtistLinkType.CharacterDesigner;
	readonly pictures: EntryPictureFileListEditStore;
	@observable releaseDate?: Date;
	@observable status: EntryStatus;
	@observable submitting = false;
	@observable updateNotes = '';
	@observable validationExpanded = false;
	readonly voiceProvider: BasicEntryLinkStore<ArtistContract>;
	readonly webLinks: WebLinksEditStore;
	readonly cultureCodes: CultureCodesEditStore;

	constructor(
		private readonly values: GlobalValues,
		antiforgeryRepo: AntiforgeryRepository,
		private readonly artistRepo: ArtistRepository,
		readonly contract: ArtistForEditContract,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				artistRepo.delete(requestToken, {
					id: contract.id,
					notes: notes,
				}),
		);

		this.baseVoicebank = new BasicEntryLinkStore<ArtistContract>((entryId) =>
			artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.illustrator = new BasicEntryLinkStore<ArtistContract>((entryId) =>
			artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.newAssociatedArtist = new BasicEntryLinkStore<ArtistContract>(
			(entryId) =>
				artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.voiceProvider = new BasicEntryLinkStore<ArtistContract>((entryId) =>
			artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		this.artistType = contract.artistType;
		this.associatedArtists = contract.associatedArtists.map(
			(artist) => new ArtistForArtistEditStore(artist),
		);
		this.baseVoicebank.id = contract.baseVoicebank?.id;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = new EnglishTranslatedStringEditStore(
			contract.description,
		);
		this.groups = contract.groups;
		this.illustrator.id = contract.illustrator?.id;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.pictures = new EntryPictureFileListEditStore(contract.pictures);
		this.releaseDate = contract.releaseDate
			? dayjs(contract.releaseDate).toDate()
			: undefined;
		this.status = contract.status;
		this.voiceProvider.id = contract.voiceProvider?.id;
		this.webLinks = new WebLinksEditStore(
			contract.webLinks,
			Object.values(WebLinkCategory),
		);
		this.cultureCodes = new CultureCodesEditStore(contract.cultureCodes);

		reaction(() => this.newAssociatedArtist.entry, this.addAssociatedArtist);
	}

	private canHaveBaseVoicebank = (at: ArtistType): boolean => {
		return (
			(ArtistHelper.isVocalistType(at) || at === ArtistType.OtherIndividual) &&
			at !== ArtistType.Vocalist
		);
	};

	private canHaveCultureCodes = (at: ArtistType): boolean => {
		return ArtistHelper.isVoiceSynthesizerType(at) || at === ArtistType.Unknown;
	};

	@computed get allowBaseVoicebank(): boolean {
		return this.canHaveBaseVoicebank(this.artistType);
	}

	@computed get allowCultureCodes(): boolean {
		return this.canHaveCultureCodes(this.artistType);
	}

	@computed get canHaveCircles(): boolean {
		return this.artistType !== ArtistType.Label;
	}

	@computed get canHaveRelatedArtists(): boolean {
		return ArtistHelper.canHaveChildVoicebanks(this.artistType);
	}

	@computed get canHaveReleaseDate(): boolean {
		const vocaloidTypes = [
			ArtistType.Vocaloid,
			ArtistType.UTAU,
			ArtistType.CeVIO,
			ArtistType.OtherVoiceSynthesizer,
			ArtistType.SynthesizerV,
			ArtistType.NEUTRINO,
			ArtistType.VoiSona,
			ArtistType.NewType,
			ArtistType.Voiceroid,
			ArtistType.VOICEVOX,
			ArtistType.ACEVirtualSinger,
			ArtistType.AIVOICE,
		];
		return vocaloidTypes.includes(this.artistType);
	}

	@computed get validationError_needReferences(): boolean {
		return (
			(!this.description.original || this.description.original.length === 0) &&
			this.webLinks.items.length === 0
		);
	}

	@computed get validationError_needType(): boolean {
		return this.artistType === ArtistType.Unknown;
	}

	@computed get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName();
	}

	@computed get validationError_unnecessaryPName(): boolean {
		const allNames = this.names.getAllNames();
		return allNames.some((n) =>
			allNames.some(
				(n2) =>
					n !== n2 &&
					(n.value === n2.value + 'P' ||
						n.value === n2.value + '-P' ||
						n.value === n2.value + 'p' ||
						n.value === n2.value + '-p'),
			),
		);
	}

	@computed get hasValidationErrors(): boolean {
		return (
			this.validationError_needReferences ||
			this.validationError_needType ||
			this.validationError_unspecifiedNames ||
			this.validationError_unnecessaryPName
		);
	}

	@action addAssociatedArtist = (): void => {
		if (!this.newAssociatedArtist.entry) return;

		this.associatedArtists.push(
			new ArtistForArtistEditStore({
				parent: this.newAssociatedArtist.entry,
				linkType: this.newAssociatedArtistType,
			}),
		);

		this.newAssociatedArtist.clear();
	};

	addGroup = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.groups.push({ id: 0, parent: artist });
		});
	};

	@action removeGroup = (group: ArtistForArtistContract): void => {
		pull(this.groups, group);
	};

	@action submit = async (
		requestToken: string,
		coverPicUpload: File | undefined,
		pictureUpload: File[],
	): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.artistRepo.edit(
				requestToken,
				{
					artistType: this.artistType,
					associatedArtists: this.associatedArtists.map((a) => a.toContract()),
					baseVoicebank: this.baseVoicebank.entry,
					defaultNameLanguage: this.defaultNameLanguage,
					description: this.description.toContract(),
					id: this.contract.id,
					illustrator: this.illustrator.entry,
					groups: this.groups,
					names: this.names.toContracts(),
					pictures: this.pictures.toContracts(),
					releaseDate: this.releaseDate?.toISOString(),
					status: this.status,
					updateNotes: this.updateNotes,
					voiceProvider: this.voiceProvider.entry,
					webLinks: this.webLinks.toContracts(),
					cultureCodes: this.cultureCodes
						.toContracts()
						.map((c) => c.toString()),
				},
				coverPicUpload,
				pictureUpload,
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
}
