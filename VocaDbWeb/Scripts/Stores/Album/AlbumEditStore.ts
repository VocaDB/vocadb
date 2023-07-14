import { AlbumForEditContract } from '@/DataContracts/Album/AlbumForEditContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { EntryStatus } from '@/Models/EntryStatus';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { PVRepository } from '@/Repositories/PVRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AlbumDiscPropertiesListEditStore } from '@/Stores/Album/AlbumDiscPropertiesListEditStore';
import { AlbumArtistRolesEditStore } from '@/Stores/Artist/AlbumArtistRolesEditStore';
import { ArtistForAlbumEditStore } from '@/Stores/ArtistForAlbumEditStore';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { CustomNameEditStore } from '@/Stores/CustomNameEditStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { EntryPictureFileListEditStore } from '@/Stores/EntryPictureFileListEditStore';
import { EnglishTranslatedStringEditStore } from '@/Stores/Globalization/EnglishTranslatedStringEditStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { PVListEditStore } from '@/Stores/PVs/PVListEditStore';
import { SongInAlbumEditStore } from '@/Stores/SongInAlbumEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import dayjs, { Dayjs } from 'dayjs';
import { isEmpty, isNumber, pull, some } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

// Single artist selection for the track properties dialog.
export class TrackArtistSelectionStore {
	// Whether this artist has been selected.
	// eslint-disable-next-line prettier/prettier
	@observable selected: boolean;

	constructor(
		readonly artist: ArtistContract,
		selected: boolean,
		private readonly filter: () => string /* TODO */,
	) {
		makeObservable(this);

		this.selected = selected;
	}

	// Whether this selection is visible according to current filter.
	get visible(): boolean {
		var f = this.filter();
		if (f.length === 0) return true;

		f = f.trim().toLowerCase();

		return (
			this.artist.name.toLowerCase().indexOf(f) >= 0 ||
			this.artist.additionalNames!.toLowerCase().indexOf(f) >= 0
		);
	}
}

// Store for the track properties dialog, for editing artists for one or more tracks.
export class TrackPropertiesStore {
	// Selectable artists.
	readonly artistSelections: TrackArtistSelectionStore[];
	// Artist filter string.
	@observable filter = '';

	constructor(artists: ArtistContract[], readonly song?: SongInAlbumEditStore) {
		makeObservable(this);

		this.artistSelections = artists.map(
			(a) =>
				new TrackArtistSelectionStore(
					a,
					song !== undefined && song.artists.some((sa) => a.id === sa.id),
					() => this.filter,
				),
		);
	}

	// At least one artist selected for this track.
	get somethingSelected(): boolean {
		return this.artistSelections.some((a) => a.selected);
	}

	// At least one artist selectable (not selected and visible).
	get somethingSelectable(): boolean {
		return this.artistSelections.some((a) => !a.selected && a.visible);
	}
}

export class AlbumEditStore {
	// Whether all tracks should be selected.
	@observable allTracksSelected: boolean;
	// List of artist links for this album.
	@observable artistLinks: ArtistForAlbumEditStore[];
	readonly artistRolesEditStore: AlbumArtistRolesEditStore;
	@observable catalogNumber: string;
	@observable defaultNameLanguage: string;
	readonly deleteStore: DeleteEntryStore;
	readonly description: EnglishTranslatedStringEditStore;
	// Album disc type.
	@observable discType: AlbumType;
	readonly discs: AlbumDiscPropertiesListEditStore;
	readonly editedArtistLink = new CustomNameEditStore();
	// State for the song being edited in the properties dialog.
	@observable editedSong?: TrackPropertiesStore;
	@observable errors?: Record<string, string[]>;
	readonly hasCover: boolean;
	@observable identifiers: string[];
	readonly names: NamesEditStore;
	@observable newIdentifier = '';
	readonly pictures: EntryPictureFileListEditStore;
	readonly pvs: PVListEditStore;
	@observable releaseDay?: number;
	readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable releaseEvents: BasicEntryLinkStore<ReleaseEventContract>[];
	@observable releaseMonth?: number;
	@observable releaseYear?: number;
	@observable status: EntryStatus;
	@observable submitting = false;
	// Buttons for the track properties dialog.
	// Whether the track properties dialog should be visible.
	@observable trackPropertiesDialogVisible: boolean;
	// List of tracks for this album.
	@observable tracks: SongInAlbumEditStore[];
	@observable updateNotes = '';
	// List of external links for this album.
	readonly webLinks: WebLinksEditStore;
	@observable validationExpanded = false;

	constructor(
		private readonly values: GlobalValues,
		antiforgeryRepo: AntiforgeryRepository,
		private readonly albumRepo: AlbumRepository,
		private readonly songRepo: SongRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		readonly eventRepo: ReleaseEventRepository,
		urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string | undefined },
		webLinkCategories: WebLinkCategory[],
		readonly contract: AlbumForEditContract,
		canBulkDeletePVs: boolean,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				albumRepo.delete(requestToken, {
					id: contract.id,
					notes: notes,
				}),
		);

		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			(entryId) => eventRepo.getOne({ id: entryId }),
		);

		this.releaseEvents = contract.originalRelease.releaseEvents.map((e) => {
			const store = new BasicEntryLinkStore<ReleaseEventContract>(
				(entryId) => eventRepo.getOne({ id: entryId})
			)
			store.id = e.id;
				
			return store;
		})

		if (this.releaseEvents.length === 0) {
			this.addReleaseEvent()
		}

		this.catalogNumber = contract.originalRelease.catNum;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = new EnglishTranslatedStringEditStore(
			contract.description,
		);
		this.discType = contract.discType;
		this.pvs = new PVListEditStore(
			pvRepo,
			urlMapper,
			contract.pvs,
			canBulkDeletePVs,
			true,
			false,
		);
		this.releaseDay = contract.originalRelease.releaseDate?.day;
		this.releaseMonth = contract.originalRelease.releaseDate?.month;
		this.releaseYear = contract.originalRelease.releaseDate?.year;
		this.releaseEvent.id = contract.originalRelease.releaseEvent?.id;
		this.status = contract.status;

		this.allTracksSelected = false;

		reaction(
			() => this.allTracksSelected,
			(selected) => {
				for (const s of this.tracks) {
					if (!s.isCustomTrack) s.selected = selected;
				}
			},
		);

		this.artistLinks = contract.artistLinks.map(
			(artist) => new ArtistForAlbumEditStore(artist),
		);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);

		this.discs = new AlbumDiscPropertiesListEditStore(contract.discs);

		this.editedSong = undefined;

		this.hasCover = contract.coverPictureMime !== undefined;

		this.identifiers = contract.identifiers;

		this.names = NamesEditStore.fromContracts(contract.names);

		this.pictures = new EntryPictureFileListEditStore(contract.pictures);

		this.trackPropertiesDialogVisible = false;

		this.tracks = contract.songs.map((song) => new SongInAlbumEditStore(song));

		for (const song of this.tracks) {
			reaction(() => song.isNextDisc, this.updateTrackNumbers);
		}

		reaction(
			() => this.tracks.map((track) => ({ isNextDisc: track.isNextDisc })),
			this.updateTrackNumbers,
		);

		this.webLinks = new WebLinksEditStore(contract.webLinks, webLinkCategories);
	}

	@computed get validationError_duplicateArtist(): boolean {
		return some(
			this.artistLinks.groupBy(
				(a) => (a.artist ? a.artist.id.toString() : a.name) + a.isSupport,
			),
			(a) => a.length > 1,
		);
	}

	@computed get validationError_needArtist(): boolean {
		return isEmpty(this.artistLinks);
	}

	@computed get validationError_needCover(): boolean {
		return !this.hasCover;
	}

	@computed get validationError_needReferences(): boolean {
		return (
			isEmpty(this.description.original) &&
			isEmpty(this.webLinks.items) &&
			isEmpty(this.pvs.pvs)
		);
	}

	@computed get validationError_needReleaseYear(): boolean {
		const num = !isNumber(this.releaseYear) || this.releaseYear === undefined;
		return num;
	}

	@computed get validationError_needTracks(): boolean {
		return this.discType !== AlbumType.Artbook && isEmpty(this.tracks);
	}

	@computed get validationError_needType(): boolean {
		return this.discType === AlbumType.Unknown;
	}

	@computed get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName();
	}

	@computed get hasValidationErrors(): boolean {
		return (
			this.validationError_duplicateArtist ||
			this.validationError_needArtist ||
			this.validationError_needCover ||
			this.validationError_needReferences ||
			this.validationError_needReleaseYear ||
			this.validationError_needTracks ||
			this.validationError_needType ||
			this.validationError_unspecifiedNames
		);
	}

	@computed get eventDate(): Dayjs | undefined {
		return this.releaseEvents
			.map(e => e.entry)
			.filter(e => e !== undefined)
			.sort(
				(a, b) =>
					(a!.date ? new Date(a!.date).getTime() : Infinity) -
					(b!.date ? new Date(b!.date).getTime() : Infinity),
			).map(e => dayjs(e!.date))[0]
	}

	@computed get releaseDate(): Dayjs | undefined {
		return this.releaseYear && this.releaseMonth && this.releaseDay
			? dayjs()
					.year(this.releaseYear)
					.month(this.releaseMonth)
					.date(this.releaseDay)
			: undefined;
	}
	set releaseDate(value: Dayjs | undefined) {
		this.releaseYear = value?.year();
		this.releaseMonth = value ? value.month() + 1 : undefined;
		this.releaseDay = value?.date();
	}
	
	addReleaseEvent = (): void =>  {
		this.releaseEvents.push(
			new BasicEntryLinkStore<ReleaseEventContract>((entryId) =>
				this.eventRepo.getOne({ id: entryId }),
			)
		)
	}

	@action acceptTrackSelection = async (
		songId?: number,
		songName?: string,
		itemType?: string,
	): Promise<void> => {
		if (songId) {
			const song = await this.songRepo.getOneWithComponents({
				id: songId,
				fields: [SongOptionalField.AdditionalNames, SongOptionalField.Artists],
				lang: this.values.languagePreference,
			});

			const artists = song
				.artists!.map((artistLink) => artistLink.artist!)
				.filter((artist) => artist != null);

			const track = new SongInAlbumEditStore({
				artists: artists,
				artistString: song.artistString,
				songAdditionalNames: song.additionalNames,
				songId: song.id,
				songName: song.name,
				discNumber: 1,
				songInAlbumId: 0,
				trackNumber: 1,
			});
			reaction(() => track.isNextDisc, this.updateTrackNumbers);
			runInAction(() => {
				this.tracks.push(track);
			});
		} else {
			const track = new SongInAlbumEditStore({
				songName: songName!,
				artists: [],
				artistString: '',
				discNumber: 1,
				songAdditionalNames: '',
				songId: 0,
				songInAlbumId: 0,
				trackNumber: 1,
				isCustomTrack: itemType === 'custom',
			});
			reaction(() => track.isNextDisc, this.updateTrackNumbers);
			this.tracks.push(track);
		}
	};

	// Adds a new artist to the album
	// artistId: Id of the artist being added, if it's an existing artist. Can be null, if custom artist.
	// customArtistName: Name of the custom artist being added. Can be null, if existing artist.
	@action addArtist = (artistId?: number, customArtistName?: string): void => {
		if (artistId) {
			this.artistRepo
				.getOne({ id: artistId, lang: this.values.languagePreference })
				.then((artist) => {
					const data: ArtistForAlbumContract = {
						artist: artist,
						isSupport: false,
						name: artist.name,
						id: 0,
						roles: 'Default' /* TODO: enum */,
					};

					const link = new ArtistForAlbumEditStore(data);
					runInAction(() => {
						this.artistLinks.push(link);
					});
				});
		} else {
			const data: ArtistForAlbumContract = {
				artist: null!,
				name: customArtistName,
				isSupport: false,
				id: 0,
				roles: 'Default' /* TODO: enum */,
			};

			const link = new ArtistForAlbumEditStore(data);
			this.artistLinks.push(link);
		}
	};

	// Adds a list of artists (from the track properties view model) to selected tracks.
	@action addArtistsToSelectedTracks = (): void => {
		for (const song of this.tracks.filter((s) => s.selected)) {
			const added = this.editedSong!.artistSelections.filter(
				(a) => a.selected && song.artists.every((a2) => a.artist.id !== a2.id),
			).map((a3) => a3.artist);
			song.artists.push(...added);
		}

		this.trackPropertiesDialogVisible = false;
	};

	@action createNewIdentifier = (): void => {
		if (!this.newIdentifier) return;

		this.identifiers.push(this.newIdentifier);
		this.newIdentifier = '';
	};

	customizeName = (artistLink: ArtistForAlbumEditStore): void => {
		this.editedArtistLink.open(artistLink);
	};

	editArtistRoles = (artist: ArtistForAlbumEditStore): void => {
		this.artistRolesEditStore.show(artist);
	};

	private artistsForTracks = (): ArtistContract[] => {
		const notAllowedTypes = ['Label'];
		return this.artistLinks
			.filter(
				(a) =>
					a.artist != null && !notAllowedTypes.includes(a.artist.artistType),
			)
			.map((a) => a.artist);
	};

	@action editMultipleTrackProperties = (): void => {
		const artists = this.artistsForTracks();
		this.editedSong = new TrackPropertiesStore(artists, undefined);
		this.trackPropertiesDialogVisible = true;
	};

	@action editTrackProperties = (song: SongInAlbumEditStore): void => {
		const artists = this.artistsForTracks();
		this.editedSong = new TrackPropertiesStore(artists, song);
		this.trackPropertiesDialogVisible = true;
	};

	// Removes an artist from this album.
	@action removeArtist = (artistForAlbum: ArtistForAlbumEditStore): void => {
		pull(this.artistLinks, artistForAlbum);
	};

	// Removes artists (selected from the track properties view model) from selected tracks.
	@action removeArtistsFromSelectedTracks = (): void => {
		for (const song of this.tracks.filter((s) => s.selected)) {
			const removed = song.artists.filter((a) =>
				this.editedSong!.artistSelections.some(
					(a2) => a2.selected && a.id === a2.artist.id,
				),
			);
			pull(song.artists, ...removed);
		}

		this.trackPropertiesDialogVisible = false;
	};

	@action removeIdentifier = (identifier: string): void => {
		pull(this.identifiers, identifier);
	};

	// Removes a track from this album.
	@action removeTrack = (song: SongInAlbumEditStore): void => {
		pull(this.tracks, song);
	};

	// Copies modified state from track properties view model to the single track being edited.
	@action saveTrackProperties = (): void => {
		this.trackPropertiesDialogVisible = false;

		if (this.editedSong) {
			const selected = this.editedSong.artistSelections
				.filter((a) => a.selected)
				.map((a) => a.artist);
			this.editedSong.song!.artists = selected;
			this.editedSong = undefined;
		}
	};

	@action submit = async (
		requestToken: string,
		coverPicUpload: File | undefined,
		pictureUpload: File[],
	): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.albumRepo.edit(
				requestToken,
				{
					artistLinks: this.artistLinks.map((artist) => artist.toContract()),
					defaultNameLanguage: this.defaultNameLanguage,
					description: this.description.toContract(),
					discs: this.discs.toContracts(),
					discType: this.discType,
					id: this.contract.id,
					identifiers: this.identifiers,
					names: this.names.toContracts(),
					originalRelease: {
						catNum: this.catalogNumber,
						releaseDate: {
							day: this.releaseDay,
							month: this.releaseMonth,
							year: this.releaseYear,
						},
						releaseEvent: this.releaseEvent.entry,
						releaseEvents: this.releaseEvents.map(e => e.entry).filter(e => e !== undefined) as ReleaseEventContract[]
					},
					pictures: this.pictures.toContracts(),
					pvs: this.pvs.toContracts(),
					songs: this.tracks,
					status: this.status,
					updateNotes: this.updateNotes,
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

	@action updateTrackNumbers = (): void => {
		let track = 1;
		let disc = 1;

		for (const song of this.tracks) {
			if (song.isNextDisc) {
				disc++;
				track = 1;
			}

			song.discNumber = disc;
			song.trackNumber = track;
			track++;
		}
	};
}
