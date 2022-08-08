import AlbumForApiContract from '@/DataContracts/Album/AlbumForApiContract';
import ArtistApiContract from '@/DataContracts/Artist/ArtistApiContract';
import CommentContract from '@/DataContracts/CommentContract';
import EnglishTranslatedStringContract from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import PVContract from '@/DataContracts/PVs/PVContract';
import ReleaseEventContract from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import ArtistForSongContract from '@/DataContracts/Song/ArtistForSongContract';
import LyricsForSongContract from '@/DataContracts/Song/LyricsForSongContract';
import SongApiContract from '@/DataContracts/Song/SongApiContract';
import SongListBaseContract from '@/DataContracts/SongListBaseContract';
import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';
import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';
import WebLinkContract from '@/DataContracts/WebLinkContract';

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
