import SongType from '@/Models/Songs/SongType';

import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import WebLinkContract from '../WebLinkContract';
import LyricsForSongContract from './LyricsForSongContract';
import SongContract from './SongContract';

// Corresponds to the SongForEditForApiContract record class in C#.
export default interface SongForEditContract {
	albumEventId?: number;
	albumReleaseDate?: string;
	artists: ArtistForAlbumContract[];
	canDelete?: boolean;
	defaultNameLanguage: string;
	deleted: boolean;
	hasAlbums: boolean;
	id: number;
	lengthSeconds: number;
	lyrics: LyricsForSongContract[];
	maxMilliBpm?: number;
	minMilliBpm?: number;
	name?: string;
	names: LocalizedStringWithIdContract[];
	notes: EnglishTranslatedStringContract;
	originalVersion?: SongContract;
	// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
	publishDate?: string;
	pvs: PVContract[];
	releaseEvent?: ReleaseEventContract;
	songType: SongType;
	status: string;
	tags: number[];
	updateNotes?: string;
	webLinks: WebLinkContract[];
}
