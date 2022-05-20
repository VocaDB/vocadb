import _ from 'lodash';
import { computed, makeObservable, observable } from 'mobx';

import AlbumForEditContract from '../../DataContracts/Album/AlbumForEditContract';
import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import AlbumType from '../../Models/Albums/AlbumType';
import PVRepository from '../../Repositories/PVRepository';
import AlbumArtistRolesEditStore from '../Artist/AlbumArtistRolesEditStore';
import ArtistForAlbumEditStore from '../ArtistForAlbumEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
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
		private readonly artist: ArtistContract,
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
			this.artist.additionalNames.toLowerCase().indexOf(f) >= 0
		);
	}
}

// Store for the track properties dialog, for editing artists for one or more tracks.
export class TrackPropertiesStore {
	// Selectable artists.
	public readonly artistSelections: TrackArtistSelectionStore[];
	// Artist filter string.
	@observable public filter = '';

	public constructor(artists: ArtistContract[], song: SongInAlbumEditStore) {
		makeObservable(this);

		this.artistSelections = _.map(
			artists,
			(a) =>
				new TrackArtistSelectionStore(
					a,
					song !== undefined && _.some(song.artists, (sa) => a.id === sa.id),
					() => this.filter,
				),
		);
	}

	// At least one artist selected for this track.
	public get somethingSelected(): boolean {
		return _.some(this.artistSelections, (a) => a.selected);
	}

	// At least one artist selectable (not selected and visible).
	public get somethingSelectable(): boolean {
		return _.some(this.artistSelections, (a) => !a.selected && a.visible);
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
	public readonly description: EnglishTranslatedStringEditStore;
	// Album disc type.
	@observable public discType: AlbumType;
	public readonly discs: AlbumDiscPropertiesListEditStore;
	// State for the song being edited in the properties dialog.
	@observable public editedSong?: TrackPropertiesStore;
	public readonly hasCover: boolean;
	public readonly id: number;
	@observable public identifiers: string[];
	public readonly names: NamesEditStore;
	@observable public newIdentifier = '';
	public readonly pictures: EntryPictureFileListEditStore;
	public readonly pvs: PVListEditStore;
	@observable public releaseDay: number;
	public readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable public releaseMonth: number;
	@observable public releaseYear: number;
	@observable public status: string;
	@observable public submittedJson = '';
	@observable public submitting = false;
	// Buttons for the track properties dialog.
	@observable public trackPropertiesDialogButtons: any[];
	// Whether the track properties dialog should be visible.
	@observable public trackPropertiesDialogVisible: boolean;
	// List of tracks for this album.
	public readonly tracks: SongInAlbumEditStore[];
	@observable public updateNotes = '';
	// List of external links for this album.
	public readonly webLinks: WebLinksEditStore;
	@observable public validationExpanded = false;

	public constructor(
		pvRepo: PVRepository,
		artistRoleNames: { [key: string]: string },
		data: AlbumForEditContract,
	) {
		makeObservable(this);

		this.catalogNumber = data.originalRelease.catNum;
		this.defaultNameLanguage = data.defaultNameLanguage;
		this.description = new EnglishTranslatedStringEditStore(data.description);
		this.discType = data.discType;
		this.id = data.id;
		this.pvs = new PVListEditStore(pvRepo);
		this.releaseDay = data.originalRelease.releaseDate?.day;
		this.releaseMonth = data.originalRelease.releaseDate?.month;
		this.releaseYear = data.originalRelease.releaseDate?.year;
		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			/* TODO */
			undefined,
		);
		this.status = data.status;

		// TODO

		this.allTracksSelected = false;

		this.artistLinks = _.map(
			data.artistLinks,
			(artist) => new ArtistForAlbumEditStore(artist),
		);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);

		this.discs = new AlbumDiscPropertiesListEditStore(data.discs);

		this.editedSong = undefined;

		this.hasCover = data.coverPictureMime !== undefined;

		this.identifiers = data.identifiers;

		this.names = NamesEditStore.fromContracts(data.names);

		this.pictures = new EntryPictureFileListEditStore(data.pictures);

		this.trackPropertiesDialogButtons = [
			{ text: 'Save' /* TODO: localize */, click: this.saveTrackProperties },
		];

		this.trackPropertiesDialogVisible = false;

		this.tracks = _.map(data.songs, (song) => new SongInAlbumEditStore(song));

		this.webLinks = new WebLinksEditStore(data.webLinks, webLinkCategories);
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
}
