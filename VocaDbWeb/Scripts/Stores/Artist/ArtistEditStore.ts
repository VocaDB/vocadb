import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForArtistContract } from '@/DataContracts/Artist/ArtistForArtistContract';
import { ArtistForEditContract } from '@/DataContracts/Artist/ArtistForEditContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistLinkType } from '@/Models/Artists/ArtistLinkType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EntryPictureFileListEditStore } from '@/Stores/EntryPictureFileListEditStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import $ from 'jquery';
import { pull } from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

export class ArtistForArtistEditStore {
	@observable public linkType: string /* TODO: enum */;
	public parent: ArtistContract;

	public constructor(link: ArtistForArtistContract) {
		makeObservable(this);

		this.linkType = link.linkType!;
		this.parent = link.parent;
	}

	public toContract = (): ArtistForArtistContract => {
		return {
			linkType: this.linkType,
			parent: this.parent,
		};
	};
}

export class ArtistEditStore {
	@observable public artistType: ArtistType;
	@observable public associatedArtists: ArtistForArtistEditStore[];
	public readonly baseVoicebank: BasicEntryLinkStore<ArtistContract>;
	@observable public defaultNameLanguage: string;
	public readonly deleteStore = new DeleteEntryStore(async (notes) => {
		await $.ajax(
			this.urlMapper.mapRelative(
				`api/artists/${this.contract.id}?notes=${encodeURIComponent(notes)}`,
			),
			{
				type: 'DELETE',
				success: () => {
					window.location.href = this.urlMapper.mapRelative(
						EntryUrlMapper.details(EntryType.Artist, this.contract.id),
					);
				},
			},
		);
	});
	public readonly description: EnglishTranslatedStringEditStore;
	@observable public errors?: Record<string, string[]>;
	@observable public groups: ArtistForArtistContract[];
	public readonly illustrator: BasicEntryLinkStore<ArtistContract>;
	public readonly names: NamesEditStore;
	public readonly newAssociatedArtist: BasicEntryLinkStore<ArtistContract>;
	@observable public newAssociatedArtistType = ArtistLinkType.CharacterDesigner;
	public readonly pictures: EntryPictureFileListEditStore;
	@observable public releaseDate?: Date;
	@observable public status: EntryStatus;
	@observable public submitting = false;
	@observable public updateNotes = '';
	@observable public validationExpanded = false;
	public readonly voiceProvider: BasicEntryLinkStore<ArtistContract>;
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
		private readonly urlMapper: UrlMapper,
		public readonly contract: ArtistForEditContract,
	) {
		makeObservable(this);

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
			? moment(contract.releaseDate).toDate()
			: undefined;
		this.status = contract.status;
		this.voiceProvider.id = contract.voiceProvider?.id;
		this.webLinks = new WebLinksEditStore(
			contract.webLinks,
			Object.values(WebLinkCategory),
		);

		reaction(() => this.newAssociatedArtist.entry, this.addAssociatedArtist);
	}

	private canHaveBaseVoicebank = (at: ArtistType): boolean => {
		return (
			(ArtistHelper.isVocalistType(at) || at === ArtistType.OtherIndividual) &&
			at !== ArtistType.Vocalist
		);
	};

	@computed public get allowBaseVoicebank(): boolean {
		return this.canHaveBaseVoicebank(this.artistType);
	}

	@computed public get canHaveCircles(): boolean {
		return this.artistType !== ArtistType.Label;
	}

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
		return vocaloidTypes.includes(this.artistType);
	}

	@computed public get validationError_needReferences(): boolean {
		return (
			(!this.description.original || this.description.original.length === 0) &&
			this.webLinks.items.length === 0
		);
	}

	@computed public get validationError_needType(): boolean {
		return this.artistType === ArtistType.Unknown;
	}

	@computed public get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName();
	}

	@computed public get validationError_unnecessaryPName(): boolean {
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

	@computed public get hasValidationErrors(): boolean {
		return (
			this.validationError_needReferences ||
			this.validationError_needType ||
			this.validationError_unspecifiedNames ||
			this.validationError_unnecessaryPName
		);
	}

	@action public addAssociatedArtist = (): void => {
		if (!this.newAssociatedArtist.entry) return;

		this.associatedArtists.push(
			new ArtistForArtistEditStore({
				parent: this.newAssociatedArtist.entry,
				linkType: this.newAssociatedArtistType,
			}),
		);

		this.newAssociatedArtist.clear();
	};

	public addGroup = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.groups.push({ id: 0, parent: artist });
		});
	};

	@action public removeGroup = (group: ArtistForArtistContract): void => {
		pull(this.groups, group);
	};

	@action public submit = async (
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
