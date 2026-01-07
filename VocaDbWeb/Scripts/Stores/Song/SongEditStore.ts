import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { SongForEditContract } from '@/DataContracts/Song/SongForEditContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { SongHelper } from '@/Helpers/SongHelper';
import { EntryStatus } from '@/Models/EntryStatus';
import { PVType } from '@/Models/PVs/PVType';
import { SongType } from '@/Models/Songs/SongType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { PVRepository } from '@/Repositories/PVRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AlbumArtistRolesEditStore } from '@/Stores/Artist/AlbumArtistRolesEditStore';
import { ArtistForAlbumEditStore } from '@/Stores/ArtistForAlbumEditStore';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { CustomNameEditStore } from '@/Stores/CustomNameEditStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { PVListEditStore } from '@/Stores/PVs/PVListEditStore';
import { SongBpmFilter } from '@/Stores/Search/SongBpmFilter';
import { SongLengthFilter } from '@/Stores/Search/SongLengthFilter';
import { LyricsForSongListEditStore } from '@/Stores/Song/LyricsForSongListEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import dayjs, { Dayjs } from 'dayjs';
import { isEmpty, pull, some, unionBy } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	observe,
	runInAction,
} from 'mobx';

import { CultureCodesEditStore } from '../CultureCodesEditStore';

interface PotentialDate {
	date: Dayjs;
	source: string;
}

export class SongEditStore {
	private readonly albumEventId?: number;
	private readonly albumReleaseDate?: Dayjs;
	// List of artist links for this song.
	// eslint-disable-next-line prettier/prettier
	@observable artistLinks: ArtistForAlbumEditStore[] = [];
	readonly artistRolesEditStore: AlbumArtistRolesEditStore;
	@observable defaultNameLanguage: string; /* TODO: enum */
	readonly deleteStore: DeleteEntryStore;
	readonly editedArtistLink = new CustomNameEditStore();
	@observable errors?: Record<string, string[]>;
	readonly hasAlbums: boolean;
	@observable hasMaxMilliBpm: boolean;
	readonly lengthFilter = new SongLengthFilter();
	readonly lyrics: LyricsForSongListEditStore;
	readonly maxBpmFilter = new SongBpmFilter();
	readonly minBpmFilter = new SongBpmFilter();
	readonly names: NamesEditStore;
	readonly notes: EnglishTranslatedStringEditStore;
	readonly originalVersion: BasicEntryLinkStore<SongContract>;
	@observable originalVersionSuggestions: SongContract[] = [];
	@observable publishDate?: Date;
	readonly pvs: PVListEditStore;
	readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable releaseEvents: BasicEntryLinkStore<ReleaseEventContract>[];
	@observable songType: SongType;
	@observable status: EntryStatus;
	@observable submitting = false;
	private readonly tags: number[];
	@observable updateNotes = '';
	readonly webLinks: WebLinksEditStore;
	readonly cultureCodes: CultureCodesEditStore;

	constructor(
		private readonly values: GlobalValues,
		antiforgeryRepo: AntiforgeryRepository,
		private readonly songRepo: SongRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		readonly eventRepo: ReleaseEventRepository,
		urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string | undefined },
		readonly contract: SongForEditContract,
		canBulkDeletePVs: boolean,
		private readonly instrumentalTagId: number,
		readonly albumId?: number,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				songRepo.delete(requestToken, {
					id: contract.id,
					notes: notes,
				}),
		);

		this.originalVersion = new BasicEntryLinkStore((entryId) =>
			songRepo.getOne({
				id: entryId,
				lang: values.languagePreference,
			}),
		);

		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			(entryId) => eventRepo.getOne({ id: entryId }),
		);

		this.releaseEvents = contract.releaseEvents.map((e) => {
			const store = new BasicEntryLinkStore<ReleaseEventContract>(
				(entryId) => eventRepo.getOne({ id: entryId})
			)
			store.id = e.id;
				
			return store;
		})

		if (this.releaseEvents.length === 0) {
			this.addReleaseEvent()
		}

		this.albumEventId = contract.albumEventId;
		this.albumReleaseDate = contract.albumReleaseDate
			? dayjs.utc(contract.albumReleaseDate)
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
			? dayjs(contract.publishDate).toDate()
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
		this.cultureCodes = new CultureCodesEditStore(
			contract.cultureCodes.length > 0
				? contract.cultureCodes
				: this.lyrics.original.cultureCodes[0] !== ''
				? this.lyrics.original.cultureCodes
				: [],
		);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);

		observe(this.lyrics.original.cultureCodes, (change) => {
			// eslint-disable-next-line no-new-wrappers
			this.cultureCodes.items = change.object.map((s) => new String(s));
		});
	}

	@computed get canHaveOriginalVersion(): boolean {
		return this.songType !== SongType.Original;
	}

	@computed get showInstrumentalNote(): boolean {
		return (
			this.pvs.isPossibleInstrumental &&
			this.songType !== SongType.Instrumental &&
			!this.tags.some((t) => t === this.instrumentalTagId)
		);
	}

	@computed get showLyricsNote(): boolean {
		return (
			this.songType !== SongType.Instrumental && !this.originalVersion.isEmpty
		);
	}

	@computed get validationError_duplicateArtist(): boolean {
		return some(
			this.artistLinks.groupBy((a) =>
				a.artist ? a.artist.id.toString() : a.name,
			),
			(a) => a.length > 1,
		);
	}

	@computed get validationError_needArtist(): boolean {
		return !this.artistLinks.some((a) => a.artist != null);
	}

	@computed get validationError_needOriginal(): boolean {
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

	@computed get validationError_needProducer(): boolean {
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

	@computed get validationError_needReferences(): boolean {
		return (
			!this.hasAlbums &&
			isEmpty(this.notes.original) &&
			isEmpty(this.webLinks.items) &&
			isEmpty(this.pvs.pvs)
		);
	}

	@computed get validationError_needType(): boolean {
		return this.songType === SongType.Unspecified;
	}

	@computed
	get validationError_nonInstrumentalSongNeedsVocalists(): boolean {
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

	@computed get validationError_redundantEvent(): boolean {
		return (
			!!this.albumEventId &&
			!this.releaseEvent.isEmpty &&
			this.releaseEvent.id === this.albumEventId
		);
	}

	@computed get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName;
	}

	@computed get validationError_needToSpecifyNames(): boolean {
		const primaryNames = [
			this.names.originalName.value.trim(),
			this.names.romajiName.value.trim(),
			this.names.englishName.value.trim(),
		].filter(name => name !== '');

		const uniqueNames = new Set(primaryNames);
		return primaryNames.length > uniqueNames.size;
	}

	@computed get hasValidationErrors(): boolean {
		return (
			this.validationError_duplicateArtist ||
			this.validationError_needArtist ||
			this.validationError_needOriginal ||
			this.validationError_needProducer ||
			this.validationError_needReferences ||
			this.validationError_needType ||
			this.validationError_nonInstrumentalSongNeedsVocalists ||
			this.validationError_redundantEvent ||
			this.validationError_unspecifiedNames ||
			this.validationError_needToSpecifyNames
		);
	}

	@computed get eventDate(): Dayjs | undefined {
		return this.releaseEvents
			.map(e => e.entry)
			.filter(e => e !== undefined && e.date !== undefined)
			.sort(
				(a, b) =>
					(a!.date ? new Date(a!.date).getTime() : Infinity) -
					(b!.date ? new Date(b!.date).getTime() : Infinity),
			).map(e => dayjs.utc(e!.date))[0]
	}

	@computed get firstPvDate(): Dayjs | undefined {
		return this.pvs.pvs
			.filter(
				(pv) => !!pv.contract.publishDate && pv.pvType === PVType.Original,
			)
			.map((pv) => dayjs.utc(pv.contract.publishDate))
			.sortBy((p) => p)
			.head();
	}

	@computed get suggestedPublishDate(): PotentialDate | undefined {
		return [
			{ date: this.albumReleaseDate, source: 'Album' },
			{ date: this.firstPvDate, source: 'PV' },
		]
			.filter((d) => d.date != null)
			.map((d) => d as PotentialDate)
			.sortBy((d) => d.date)
			.head();
	}

	addReleaseEvent = (): void =>  {
		this.releaseEvents.push(
			new BasicEntryLinkStore<ReleaseEventContract>((entryId) =>
				this.eventRepo.getOne({ id: entryId }),
			)
		)
	}

	// Adds a new artist to the album
	// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
	// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
	addArtist = async (
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

	customizeName = (artistLink: ArtistForAlbumEditStore): void => {
		this.editedArtistLink.open(artistLink);
	};

	editArtistRoles = (artist: ArtistForAlbumEditStore): void => {
		this.artistRolesEditStore.show(artist);
	};

	@action findOriginalSongSuggestions = async (): Promise<void> => {
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

		const suggestions = unionBy(originals, all, (i) => i.id).take(3);

		runInAction(() => {
			this.originalVersionSuggestions = suggestions;
		});
	};

	// Removes an artist from this album.
	@action removeArtist = (artist: ArtistForAlbumEditStore): void => {
		pull(this.artistLinks, artist);
	};

	@action selectOriginalVersion = (song: SongContract): void => {
		this.originalVersion.id = song.id;
	};

	@action submit = async (requestToken: string): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.songRepo.edit(requestToken, {
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
				releaseEvents: this.releaseEvents.map(e => e.entry).filter(e => e !== undefined) as ReleaseEventContract[],
				songType: this.songType,
				status: this.status,
				tags: this.tags,
				updateNotes: this.updateNotes,
				webLinks: this.webLinks.toContracts(),
				cultureCodes:
					this.songType === SongType.Instrumental
						? []
						: this.cultureCodes.toContracts().map((s) => s.toString()),
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
