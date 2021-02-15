import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import LyricsForSongContract from './LyricsForSongContract';
import PVContract from '../PVs/PVContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import SongContract from './SongContract';
import WebLinkContract from '../WebLinkContract';

	export default interface SongForEditContract {

		albumEventId?: number;

		albumReleaseDate?: string;

		artists: ArtistForAlbumContract[];

		defaultNameLanguage: string;

		deleted: boolean;

		hasAlbums: boolean;

		id: number;

		lengthSeconds: number;

		lyrics: LyricsForSongContract[];

		names: LocalizedStringWithIdContract[];

		notes: EnglishTranslatedStringContract;

		originalVersion: SongContract;

		// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
		publishDate?: string;

		pvs: PVContract[];

		releaseEvent?: ReleaseEventContract;

		songType: string;

		status: string;

		tags: number[];

		updateNotes?: string;

		webLinks: WebLinkContract[];

		minBpm?: number;

		maxBpm?: number;

	}