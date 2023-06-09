import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { EntryTypeAndSubTypeContract } from '@/types/DataContracts/EntryTypeAndSubTypeContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { EventSeriesContract } from '@/types/DataContracts/ReleaseEvents/EventSeriesContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongContract } from '@/types/DataContracts/Song/SongContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';

// Corresponds to the TagStatsForApiContract record class in C#.
interface TagStatsContract {
	albumCount: number;
	albums: AlbumForApiContract[];
	artistCount: number;
	artists: ArtistApiContract[];
	eventCount: number;
	events: ReleaseEventContract[];
	eventSeries: EventSeriesContract[];
	followerCount: number;
	songCount: number;
	songListCount: number;
	songs: SongContract[];
}

// Corresponds to the TagDetailsForApiContract record class in C#.
export interface TagDetailsContract {
	additionalNames?: string;
	allUsageCount: number;
	categoryName?: string;
	children: TagBaseContract[];
	commentCount: number;
	createDate: string;
	deleted: boolean;
	description: EnglishTranslatedStringContract;
	id: number;
	isFollowing: boolean;
	latestComments: CommentContract[];
	mainPicture?: EntryThumbContract;
	mappedNicoTags: string[];
	name: string;
	parent?: TagBaseContract;
	relatedEntryType: EntryTypeAndSubTypeContract;
	relatedTags: TagBaseContract[];
	siblings: TagBaseContract[];
	stats: TagStatsContract;
	status: EntryStatus;
	targets: number;
	translations: string;
	webLinks: WebLinkContract[];
}
