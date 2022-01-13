import AlbumForApiContract from '../Album/AlbumForApiContract';
import ArtistApiContract from '../Artist/ArtistApiContract';
import CommentContract from '../CommentContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import PVContract from '../PVs/PVContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import SongListBaseContract from '../SongListBaseContract';
import TagBaseContract from '../Tag/TagBaseContract';
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';
import WebLinkContract from '../WebLinkContract';
import ArtistForSongContract from './ArtistForSongContract';
import LyricsForSongContract from './LyricsForSongContract';
import SongApiContract from './SongApiContract';

// Corresponds to the SongInAlbumForApiContract in C#.
interface SongInAlbumContract {
	discNumber: number;
	song: SongApiContract;
	trackNumber: number;
}

// Corresponds to the SongDetailsForApiContract in C#.
export default interface SongDetailsContract {
	additionalNames: string;
	album?: AlbumForApiContract;
	albums: AlbumForApiContract[];
	albumSong: SongInAlbumContract;
	alternateVersions: SongApiContract[];
	artists: ArtistForSongContract[];
	artistString?: string;
	canEditPersonalDescription: boolean;
	canRemoveTagUsages: boolean;
	commentCount: number;
	createDate: Date;
	deleted: boolean;
	hits: number;
	latestComments: CommentContract[];
	likeCount: number;
	listCount: number;
	lyricsFromParents: LyricsForSongContract[];
	maxMilliBpm?: number;
	mergedTo?: SongApiContract;
	minMilliBpm?: number;
	nextSong?: SongInAlbumContract;
	notes: EnglishTranslatedStringContract;
	originalVersion?: SongApiContract;
	personalDescriptionAuthor?: ArtistApiContract;
	personalDescriptionText?: string;
	pools: SongListBaseContract[];
	preferredLyrics?: LyricsForSongContract;
	previousSong?: SongInAlbumContract;
	pvs: PVContract[];
	releaseEvent?: ReleaseEventContract;
	song: SongApiContract;
	songTypeTag: TagBaseContract;
	subjectsFromParents: ArtistForSongContract[];
	suggestions: SongApiContract[];
	tags: TagUsageForApiContract[];
	userRating: string /* TODO: enum */;
	webLinks: WebLinkContract[];
}
