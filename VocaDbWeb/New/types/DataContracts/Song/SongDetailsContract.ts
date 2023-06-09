import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ArtistForSongContract } from '@/types/DataContracts/Song/ArtistForSongContract';
import { LyricsForSongContract } from '@/types/DataContracts/Song/LyricsForSongContract';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { SongListBaseContract } from '@/types/DataContracts/SongListBaseContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';

// Corresponds to the SongInAlbumForApiContract in C#.
interface SongInAlbumContract {
	discNumber: number;
	song: SongApiContract;
	trackNumber: number;
}

// Corresponds to the SongDetailsForApiContract in C#.
export interface SongDetailsContract {
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
	createDate: string;
	cultureCodes: string[];
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
