import ArtistForAlbumContract from '@/DataContracts/ArtistForAlbumContract';
import ReleaseEventContract from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import SongContract from '@/DataContracts/Song/SongContract';
import SongForEditContract from '@/DataContracts/Song/SongForEditContract';
import ArtistHelper from '@/Helpers/ArtistHelper';
import SongHelper from '@/Helpers/SongHelper';
import EntryType from '@/Models/EntryType';
import PVType from '@/Models/PVs/PVType';
import SongType from '@/Models/Songs/SongType';
import WebLinkCategory from '@/Models/WebLinkCategory';
import ArtistRepository from '@/Repositories/ArtistRepository';
import PVRepository from '@/Repositories/PVRepository';
import ReleaseEventRepository from '@/Repositories/ReleaseEventRepository';
import SongRepository from '@/Repositories/SongRepository';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import GlobalValues from '@/Shared/GlobalValues';
import UrlMapper from '@/Shared/UrlMapper';
import $ from 'jquery';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';
import moment, { Moment } from 'moment';

import AlbumArtistRolesEditStore from '../Artist/AlbumArtistRolesEditStore';
import ArtistForAlbumEditStore from '../ArtistForAlbumEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
import CustomNameEditStore from '../CustomNameEditStore';
import DeleteEntryStore from '../DeleteEntryStore';
import EnglishTranslatedStringEditStore from '../Globalization/EnglishTranslatedStringEditStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import PVListEditStore from '../PVs/PVListEditStore';
import SongBpmFilter from '../Search/SongBpmFilter';
import SongLengthFilter from '../Search/SongLengthFilter';
import WebLinksEditStore from '../WebLinksEditStore';
import LyricsForSongListEditStore from './LyricsForSongListEditStore';

interface PotentialDate {
	date: Moment;
	source: string;
}

export default class SongEditStore {
	private readonly albumEventId?: number;
	private readonly albumReleaseDate?: Moment;
	// List of artist links for this song.
	@observable public artistLinks: ArtistForAlbumEditStore[] = [];
	public readonly artistRolesEditStore: AlbumArtistRolesEditStore;
	@observable public defaultNameLanguage: string; /* TODO: enum */
	public readonly deleteStore = new DeleteEntryStore(async (notes) => {
		await $.ajax(
			this.urlMapper.mapRelative(
				`api/songs/${this.contract.id}?notes=${encodeURIComponent(notes)}`,
			),
			{
				type: 'DELETE',
				success: () => {
					window.location.href = this.urlMapper.mapRelative(
						EntryUrlMapper.details(EntryType.Song, this.contract.id),
					);
				},
			},
		);
	});
	public readonly editedArtistLink = new CustomNameEditStore();
	@observable public errors?: Record<string, string[]>;
	public readonly hasAlbums: boolean;
	@observable public hasMaxMilliBpm: boolean;
	public readonly lengthFilter = new SongLengthFilter();
	public readonly lyrics: LyricsForSongListEditStore;
	public readonly maxBpmFilter = new SongBpmFilter();
	public readonly minBpmFilter = new SongBpmFilter();
	public readonly names: NamesEditStore;
	public readonly notes: EnglishTranslatedStringEditStore;
	public readonly originalVersion: BasicEntryLinkStore<SongContract>;
	@observable public originalVersionSuggestions: SongContract[] = [];
	@observable public publishDate?: Date;
	public readonly pvs: PVListEditStore;
	public readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable public songType: SongType;
	@observable public status: string /* TODO: enum */;
	@observable public submitting = false;
	private readonly tags: number[];
	@observable public updateNotes = '';
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly values: GlobalValues,
		private readonly songRepo: SongRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		eventRepo: ReleaseEventRepository,
		private readonly urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string | undefined },
		public readonly contract: SongForEditContract,
		canBulkDeletePVs: boolean,
		private readonly instrumentalTagId: number,
	) {
		makeObservable(this);

		this.originalVersion = new BasicEntryLinkStore((entryId) =>
			songRepo.getOne({
				id: entryId,
				lang: values.languagePreference,
			}),
		);

		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			(entryId) => eventRepo.getOne({ id: entryId }),
		);

		this.albumEventId = contract.albumEventId;
		this.albumReleaseDate = contract.albumReleaseDate
			? moment(contract.albumReleaseDate)
			: undefined;
		this.artistLinks = contract.artists.map(
			(artist) => new ArtistForAlbumEditStore(artist),
		);
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.hasAlbums = contract.hasAlbums;
		this.hasMaxMilliBpm =
			!!contract.maxMilliBpm &&
			!!contract.minMilliBpm &&
			contract.maxMilliBpm > contract.minMilliBpm;
		this.lengthFilter.length = contract.lengthSeconds;
		this.lyrics = new LyricsForSongListEditStore(contract.lyrics);
		this.maxBpmFilter.milliBpm = this.hasMaxMilliBpm
			? contract.maxMilliBpm
			: undefined;
		this.minBpmFilter.milliBpm = contract.minMilliBpm;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.notes = new EnglishTranslatedStringEditStore(contract.notes);
		this.originalVersion.id = contract.originalVersion?.id;
		this.publishDate = contract.publishDate
			? moment(contract.publishDate).toDate()
			: undefined;
		this.pvs = new PVListEditStore(
			pvRepo,
			urlMapper,
			contract.pvs,
			canBulkDeletePVs,
			true,
			true,
		);
		this.releaseEvent.id = contract.releaseEvent?.id;
		this.songType = contract.songType;
		this.status = contract.status;
		this.tags = contract.tags;
		this.webLinks = new WebLinksEditStore(
			contract.webLinks,
			Object.values(WebLinkCategory),
		);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);
	}

	@computed public get canHaveOriginalVersion(): boolean {
		return this.songType !== SongType.Original;
	}

	@computed public get showInstrumentalNote(): boolean {
		return (
			this.pvs.isPossibleInstrumental &&
			this.songType !== SongType.Instrumental &&
			!this.tags.some((t) => t === this.instrumentalTagId)
		);
	}

	@computed public get showLyricsNote(): boolean {
		return (
			this.songType !== SongType.Instrumental && !this.originalVersion.isEmpty
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
		return !this.artistLinks.some((a) => a.artist != null);
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
			(this.notes.original === null || this.notes.original === '') &&
			this.originalVersion.entry == null &&
			derivedTypes.includes(this.songType)
		);
	}

	@computed public get validationError_needProducer(): boolean {
		return (
			!this.validationError_needArtist &&
			!this.artistLinks.some(
				(a) =>
					a.artist != null &&
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
			!this.tags.some((t) => t === this.instrumentalTagId) &&
			!this.artistLinks.some((a) =>
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

	@computed public get eventDate(): Moment | undefined {
		return this.releaseEvent.entry && this.releaseEvent.entry.date
			? moment(this.releaseEvent.entry.date)
			: undefined;
	}

	@computed public get firstPvDate(): Moment {
		return _.chain(this.pvs.pvs)
			.filter((pv) => !!pv.publishDate && pv.pvType === PVType[PVType.Original])
			.map((pv) => moment(pv.publishDate))
			.sortBy((p) => p)
			.head()
			.value();
	}

	@computed public get suggestedPublishDate(): PotentialDate {
		return _.chain([
			{ date: this.albumReleaseDate, source: 'Album' },
			{ date: this.firstPvDate, source: 'PV' },
		])
			.filter((d) => d.date != null)
			.map((d) => d as PotentialDate)
			.sortBy((d) => d.date)
			.head()
			.value();
	}

	// Adds a new artist to the album
	// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
	// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
	public addArtist = async (
		artistId?: number,
		customArtistName?: string,
	): Promise<void> => {
		if (artistId) {
			const artist = await this.artistRepo.getOne({
				id: artistId,
				lang: this.values.languagePreference,
			});

			const data: ArtistForAlbumContract = {
				artist: artist,
				isSupport: false,
				name: artist.name,
				id: 0,
				roles: 'Default',
			};

			const link = new ArtistForAlbumEditStore(data);
			runInAction(() => {
				this.artistLinks.push(link);
			});
		} else {
			const data: ArtistForAlbumContract = {
				artist: undefined!,
				name: customArtistName,
				isSupport: false,
				id: 0,
				roles: 'Default',
			};

			const link = new ArtistForAlbumEditStore(data);
			runInAction(() => {
				this.artistLinks.push(link);
			});
		}
	};

	public customizeName = (artistLink: ArtistForAlbumEditStore): void => {
		this.editedArtistLink.open(artistLink);
	};

	public editArtistRoles = (artist: ArtistForAlbumEditStore): void => {
		this.artistRolesEditStore.show(artist);
	};

	@action public findOriginalSongSuggestions = async (): Promise<void> => {
		this.originalVersionSuggestions = [];

		const names = (this.names.getPrimaryNames().length
			? this.names.getPrimaryNames()
			: this.names.getAllNames()
		).map((n) => n.value);
		const [all, originals] = await Promise.all([
			this.songRepo.getByNames({
				names: names,
				ignoreIds: [this.contract.id],
				lang: this.values.languagePreference,
			}),
			this.songRepo.getByNames({
				names: names,
				ignoreIds: [this.contract.id],
				lang: this.values.languagePreference,
				songTypes: [SongType.Original, SongType.Remaster],
			}),
		]);

		const suggestions = _.chain(originals)
			.unionBy(all, (i) => i.id)
			.take(3)
			.value();

		runInAction(() => {
			this.originalVersionSuggestions = suggestions;
		});
	};

	// Removes an artist from this album.
	@action public removeArtist = (artist: ArtistForAlbumEditStore): void => {
		_.pull(this.artistLinks, artist);
	};

	@action public selectOriginalVersion = (song: SongContract): void => {
		this.originalVersion.id = song.id;
	};

	@action public submit = async (): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.songRepo.edit({
				artists: this.artistLinks.map((artistLink) => artistLink.toContract()),
				defaultNameLanguage: this.defaultNameLanguage,
				deleted: false,
				hasAlbums: this.hasAlbums,
				id: this.contract.id,
				lengthSeconds: this.lengthFilter.length,
				lyrics: this.lyrics.toContracts(),
				maxMilliBpm: this.hasMaxMilliBpm
					? this.maxBpmFilter.milliBpm
					: undefined,
				minMilliBpm: this.minBpmFilter.milliBpm,
				names: this.names.toContracts(),
				notes: this.notes.toContract(),
				originalVersion: this.originalVersion.entry,
				publishDate: this.publishDate
					? this.publishDate.toISOString()
					: undefined,
				pvs: this.pvs.toContracts(),
				releaseEvent: this.releaseEvent.entry,
				songType: this.songType,
				status: this.status,
				tags: this.tags,
				updateNotes: this.updateNotes,
				webLinks: this.webLinks.toContracts(),
			});

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
