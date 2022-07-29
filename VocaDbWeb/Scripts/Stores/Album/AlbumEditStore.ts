import AlbumForEditContract from '@DataContracts/Album/AlbumForEditContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistForAlbumContract from '@DataContracts/ArtistForAlbumContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import AlbumType from '@Models/Albums/AlbumType';
import EntryType from '@Models/EntryType';
import WebLinkCategory from '@Models/WebLinkCategory';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import PVRepository from '@Repositories/PVRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import SongRepository from '@Repositories/SongRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import UrlMapper from '@Shared/UrlMapper';
import $ from 'jquery';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment, { Moment } from 'moment';

import AlbumArtistRolesEditStore from '../Artist/AlbumArtistRolesEditStore';
import ArtistForAlbumEditStore from '../ArtistForAlbumEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
import CustomNameEditStore from '../CustomNameEditStore';
import DeleteEntryStore from '../DeleteEntryStore';
import EntryPictureFileListEditStore from '../EntryPictureFileListEditStore';
import EnglishTranslatedStringEditStore from '../Globalization/EnglishTranslatedStringEditStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import PVListEditStore from '../PVs/PVListEditStore';
import SongInAlbumEditStore from '../SongInAlbumEditStore';
import WebLinksEditStore from '../WebLinksEditStore';
import AlbumDiscPropertiesListEditStore from './AlbumDiscPropertiesListEditStore';

// Single artist selection for the track properties dialog.
export class TrackArtistSelectionStore {
	// Whether this artist has been selected.
	@observable public selected: boolean;

	public constructor(
		public readonly artist: ArtistContract,
		selected: boolean,
		private readonly filter: () => string /* TODO */,
	) {
		makeObservable(this);

		this.selected = selected;
	}

	// Whether this selection is visible according to current filter.
	public get visible(): boolean {
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
	public readonly artistSelections: TrackArtistSelectionStore[];
	// Artist filter string.
	@observable public filter = '';

	public constructor(
		artists: ArtistContract[],
		public readonly song?: SongInAlbumEditStore,
	) {
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
	public get somethingSelected(): boolean {
		return this.artistSelections.some((a) => a.selected);
	}

	// At least one artist selectable (not selected and visible).
	public get somethingSelectable(): boolean {
		return this.artistSelections.some((a) => !a.selected && a.visible);
	}
}

export default class AlbumEditStore {
	// Whether all tracks should be selected.
	@observable public allTracksSelected: boolean;
	// List of artist links for this album.
	@observable public artistLinks: ArtistForAlbumEditStore[];
	public readonly artistRolesEditStore: AlbumArtistRolesEditStore;
	@observable public catalogNumber: string;
	@observable public defaultNameLanguage: string;
	public readonly deleteStore = new DeleteEntryStore(async (notes) => {
		await $.ajax(
			this.urlMapper.mapRelative(
				`api/albums/${this.contract.id}?notes=${encodeURIComponent(notes)}`,
			),
			{
				type: 'DELETE',
				success: () => {
					window.location.href = this.urlMapper.mapRelative(
						EntryUrlMapper.details(EntryType.Album, this.contract.id),
					);
				},
			},
		);
	});
	public readonly description: EnglishTranslatedStringEditStore;
	// Album disc type.
	@observable public discType: AlbumType;
	public readonly discs: AlbumDiscPropertiesListEditStore;
	public readonly editedArtistLink = new CustomNameEditStore();
	// State for the song being edited in the properties dialog.
	@observable public editedSong?: TrackPropertiesStore;
	@observable public errors?: Record<string, string[]>;
	public readonly hasCover: boolean;
	@observable public identifiers: string[];
	public readonly names: NamesEditStore;
	@observable public newIdentifier = '';
	public readonly pictures: EntryPictureFileListEditStore;
	public readonly pvs: PVListEditStore;
	@observable public releaseDay?: number;
	public readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable public releaseMonth?: number;
	@observable public releaseYear?: number;
	@observable public status: string;
	@observable public submitting = false;
	// Buttons for the track properties dialog.
	// Whether the track properties dialog should be visible.
	@observable public trackPropertiesDialogVisible: boolean;
	// List of tracks for this album.
	@observable public tracks: SongInAlbumEditStore[];
	@observable public updateNotes = '';
	// List of external links for this album.
	public readonly webLinks: WebLinksEditStore;
	@observable public validationExpanded = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		private readonly songRepo: SongRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		eventRepo: ReleaseEventRepository,
		private readonly urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string | undefined },
		webLinkCategories: WebLinkCategory[],
		public readonly contract: AlbumForEditContract,
		canBulkDeletePVs: boolean,
	) {
		makeObservable(this);

		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			(entryId) => eventRepo.getOne({ id: entryId }),
		);

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

	@computed public get validationError_duplicateArtist(): boolean {
		return _.some(
			_.groupBy(
				this.artistLinks,
				(a) => (a.artist ? a.artist.id.toString() : a.name) + a.isSupport,
			),
			(a) => a.length > 1,
		);
	}

	@computed public get validationError_needArtist(): boolean {
		return _.isEmpty(this.artistLinks);
	}

	@computed public get validationError_needCover(): boolean {
		return !this.hasCover;
	}

	@computed public get validationError_needReferences(): boolean {
		return (
			_.isEmpty(this.description.original) &&
			_.isEmpty(this.webLinks.items) &&
			_.isEmpty(this.pvs.pvs)
		);
	}

	@computed public get validationError_needReleaseYear(): boolean {
		const num = !_.isNumber(this.releaseYear) || this.releaseYear === undefined;
		return num;
	}

	@computed public get validationError_needTracks(): boolean {
		return this.discType !== AlbumType.Artbook && _.isEmpty(this.tracks);
	}

	@computed public get validationError_needType(): boolean {
		return this.discType === AlbumType.Unknown;
	}

	@computed public get validationError_unspecifiedNames(): boolean {
		return !this.names.hasPrimaryName();
	}

	@computed public get hasValidationErrors(): boolean {
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

	@computed public get eventDate(): Moment | undefined {
		return this.releaseEvent.entry && this.releaseEvent.entry.date
			? moment(this.releaseEvent.entry.date)
			: undefined;
	}

	@computed public get releaseDate(): Moment | undefined {
		return this.releaseYear && this.releaseMonth && this.releaseDay
			? moment([this.releaseYear, this.releaseMonth, this.releaseDay])
			: undefined;
	}
	public set releaseDate(value: Moment | undefined) {
		this.releaseYear = value?.year();
		this.releaseMonth = value ? value.month() + 1 : undefined;
		this.releaseDay = value?.date();
	}

	@action public acceptTrackSelection = async (
		songId?: number,
		songName?: string,
		itemType?: string,
	): Promise<void> => {
		if (songId) {
			const song = await this.songRepo.getOneWithComponents({
				id: songId,
				fields: ['AdditionalNames', 'Artists'] /* TODO: enum */
					.join(','),
				lang: this.values.languagePreference,
			});

			const artists = song
				.artists!.map((artistLink) => artistLink.artist)
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
	@action public addArtist = (
		artistId?: number,
		customArtistName?: string,
	): void => {
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
	@action public addArtistsToSelectedTracks = (): void => {
		for (const song of this.tracks.filter((s) => s.selected)) {
			const added = this.editedSong!.artistSelections.filter(
				(a) => a.selected && song.artists.every((a2) => a.artist.id !== a2.id),
			).map((a3) => a3.artist);
			song.artists.push(...added);
		}

		this.trackPropertiesDialogVisible = false;
	};

	@action public createNewIdentifier = (): void => {
		if (!this.newIdentifier) return;

		this.identifiers.push(this.newIdentifier);
		this.newIdentifier = '';
	};

	public customizeName = (artistLink: ArtistForAlbumEditStore): void => {
		this.editedArtistLink.open(artistLink);
	};

	public editArtistRoles = (artist: ArtistForAlbumEditStore): void => {
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

	@action public editMultipleTrackProperties = (): void => {
		const artists = this.artistsForTracks();
		this.editedSong = new TrackPropertiesStore(artists, undefined);
		this.trackPropertiesDialogVisible = true;
	};

	@action public editTrackProperties = (song: SongInAlbumEditStore): void => {
		const artists = this.artistsForTracks();
		this.editedSong = new TrackPropertiesStore(artists, song);
		this.trackPropertiesDialogVisible = true;
	};

	// Removes an artist from this album.
	@action public removeArtist = (
		artistForAlbum: ArtistForAlbumEditStore,
	): void => {
		_.pull(this.artistLinks, artistForAlbum);
	};

	// Removes artists (selected from the track properties view model) from selected tracks.
	@action public removeArtistsFromSelectedTracks = (): void => {
		for (const song of this.tracks.filter((s) => s.selected)) {
			const removed = song.artists.filter((a) =>
				this.editedSong!.artistSelections.some(
					(a2) => a2.selected && a.id === a2.artist.id,
				),
			);
			_.pull(song.artists, ...removed);
		}

		this.trackPropertiesDialogVisible = false;
	};

	@action public removeIdentifier = (identifier: string): void => {
		_.pull(this.identifiers, identifier);
	};

	// Removes a track from this album.
	@action public removeTrack = (song: SongInAlbumEditStore): void => {
		_.pull(this.tracks, song);
	};

	// Copies modified state from track properties view model to the single track being edited.
	@action public saveTrackProperties = (): void => {
		this.trackPropertiesDialogVisible = false;

		if (this.editedSong) {
			const selected = this.editedSong.artistSelections
				.filter((a) => a.selected)
				.map((a) => a.artist);
			this.editedSong.song!.artists = selected;
			this.editedSong = undefined;
		}
	};

	@action public submit = async (
		coverPicUpload: File | undefined,
		pictureUpload: File[],
	): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.albumRepo.edit(
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

	@action public updateTrackNumbers = (): void => {
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
