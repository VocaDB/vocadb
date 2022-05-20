import _ from 'lodash';
import { computed, makeObservable, observable } from 'mobx';
import moment from 'moment';

import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import SongContract from '../../DataContracts/Song/SongContract';
import SongForEditContract from '../../DataContracts/Song/SongForEditContract';
import ArtistHelper from '../../Helpers/ArtistHelper';
import KnockoutHelper from '../../Helpers/KnockoutHelper';
import SongHelper from '../../Helpers/SongHelper';
import SongType from '../../Models/Songs/SongType';
import SongRepository from '../../Repositories/SongRepository';
import GlobalValues from '../../Shared/GlobalValues';
import { PotentialDate } from '../../ViewModels/Song/SongEditViewModel';
import AlbumArtistRolesEditStore from '../Artist/AlbumArtistRolesEditStore';
import ArtistForAlbumEditStore from '../ArtistForAlbumEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
import EnglishTranslatedStringEditStore from '../Globalization/EnglishTranslatedStringEditStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import PVListEditStore from '../PVs/PVListEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class SongEditStore {
	public readonly albumEventId: number;
	public readonly albumReleaseDate?: moment.Moment;
	// List of artist links for this song.
	@observable public readonly artistLinks: ArtistForAlbumEditStore[];
	@observable public defaultNameLanguage: string;
	public deleted: boolean;
	public readonly editedArtistLink = new CustomNameEditStore();
	public readonly id: number;
	@observable public length: number;
	public readonly lyrics: LyricsForSongListEditStore;
	public readonly names: NamesEditStore;
	public readonly notes: EnglishTranslatedStringEditStore;
	public readonly originalVersion: BasicEntryLinkStore<SongContract>;
	@observable public originalVersionSuggestions: SongContract[];
	@observable public publishDate?: Date;
	public readonly pvs: PVListEditStore;
	public readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable public songType: SongType;
	@observable public status: string;
	@observable public submittedJson = '';
	@observable public submitting = false;
	public readonly tags: number[];
	@observable public updateNotes = '';
	@observable public validationExpanded = false;
	public readonly webLinks: WebLinksEditStore;
	@observable public hasMaxMilliBpm: boolean;
	@observable public minMilliBpm?: number;
	@observable public maxMilliBpm?: number;
	public readonly artistRolesEditStore: AlbumArtistRolesEditStore;
	public hasAlbums: boolean;

	public constructor(
		values: GlobalValues,
		songRepo: SongRepository,
		data: SongForEditContract,
		private readonly instrumentalTagId: number,
	) {
		makeObservable(this);

		this.albumEventId = data.albumEventId;
		this.albumReleaseDate = data.albumReleaseDate
			? moment(data.albumReleaseDate)
			: undefined;
		this.artistLinks = _.map(
			data.artists,
			(artist) => new ArtistForAlbumEditStore(artist),
		);
		this.defaultNameLanguage = data.defaultNameLanguage;
		this.deleted = data.deleted;
		this.id = data.id;
		this.length = data.lengthSeconds;
		this.lyrics = new LyricsForSongListEditStore(data.lyrics);
		this.names = NamesEditStore.fromContracts(data.names);
		this.notes = new EnglishTranslatedStringEditStore(data.notes);
		this.originalVersion = new BasicEntryLinkStore<SongContract>(
			// TODO: data.originalVersion,
			(entryId) =>
				songRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);
		this.publishDate = data.publishDate
			? moment(data.publishDate).toDate()
			: undefined;
		this.pvs = new PVListEditStore(pvRepo);
		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			// TODO: data.releaseEvent,
			undefined,
		);
		this.songType = data.songType;
		this.status = data.status;
		this.tags = data.tags;
		this.webLinks = new WebLinksEditStore(data.webLinks, webLinkCategories);
		this.hasMaxMilliBpm = data.maxMilliBpm > data.minMilliBpm;
		this.minMilliBpm = data.minMilliBpm;
		this.maxMilliBpm =
			data.maxMilliBpm > data.minMilliBpm ? data.maxMilliBpm : undefined;

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);

		this.hasAlbums = data.hasAlbums;

		this.lengthFormatted = KnockoutHelper.lengthFormatted(this.length);
	}

	@computed public get canHaveOriginalVersion(): boolean {
		return this.songType !== SongType.Original;
	}

	@computed public get lengthFormatted(): string {}

	@computed public get showLyricsNote(): boolean {
		return (
			this.songType !== SongType.Instrumental && !this.originalVersion.isEmpty
		);
	}

	@computed public get suggestedPublishDate(): PotentialDate;

	@computed public get minBpm(): string {}

	@computed public get maxBpm(): string {}

	@computed public get showInstrumentalNote(): boolean {
		return (
			this.pvs.isPossibleInstrumental() &&
			this.songType !== SongType.Instrumental &&
			!_.some(this.tags, (t) => t === this.instrumentalTagId)
		);
	}

	@computed public get validationError_duplicateArtist(): boolean {
		return _.some(
			_.groupBy(this.artistLinks, (a) =>
				a.artist ? a.artist.id.toString() : a.name,
			),
			(a) => a.length > 1,
		);
	}

	@computed public get validationError_needArtist(): boolean {
		return !_.some(this.artistLinks, (a) => a.artist !== undefined);
	}

	@computed public get validationError_needOriginal(): boolean {
		const derivedTypes = [
			SongType.Remaster,
			SongType.Cover,
			SongType.Instrumental,
			SongType.MusicPV,
			SongType.Other,
			SongType.Remix,
			SongType.Arrangement,
		];
		return (
			(this.notes.original === undefined || this.notes.original === '') &&
			this.originalVersion.entry === undefined &&
			_.includes(derivedTypes, this.songType)
		);
	}

	@computed public get validationError_needProducer(): boolean {
		return (
			!this.validationError_needArtist &&
			!_.some(
				this.artistLinks,
				(a) =>
					a.artist !== undefined &&
					ArtistHelper.isProducerRole(
						a.artist,
						a.rolesArrayTyped,
						SongHelper.getContentFocus(this.songType),
					),
			)
		);
	}

	@computed public get validationError_needReferences(): boolean {
		return (
			!this.hasAlbums &&
			_.isEmpty(this.notes.original) &&
			_.isEmpty(this.webLinks.items) &&
			_.isEmpty(this.pvs.pvs)
		);
	}

	@computed public get validationError_needType(): boolean {
		return this.songType === SongType.Unspecified;
	}

	@computed
	public get validationError_nonInstrumentalSongNeedsVocalists(): boolean {
		return (
			!this.validationError_needArtist &&
			!SongHelper.isInstrumental(this.songType) &&
			this.songType !== SongType.Arrangement && // Arrangements are considered possible instrumentals in this context
			!_.some(this.tags, (t) => t === this.instrumentalTagId) &&
			!_.some(this.artistLinks, (a) =>
				ArtistHelper.isVocalistRole(a.artist, a.rolesArrayTyped),
			)
		);
	}

	@computed public get validationError_redundantEvent(): boolean {
		return (
			!!this.albumEventId &&
			!this.releaseEvent.isEmpty &&
			this.releaseEvent.id === this.albumEventId
		);
	}

	@computed public get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName;
	}

	@computed public get hasValidationErrors(): boolean {
		return (
			this.validationError_duplicateArtist ||
			this.validationError_needArtist ||
			this.validationError_needOriginal ||
			this.validationError_needProducer ||
			this.validationError_needReferences ||
			this.validationError_needType ||
			this.validationError_nonInstrumentalSongNeedsVocalists ||
			this.validationError_redundantEvent ||
			this.validationError_unspecifiedNames
		);
	}
}
