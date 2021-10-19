import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import ArtistApiContract from '@DataContracts/Artist/ArtistApiContract';
import CommentContract from '@DataContracts/CommentContract';
import EntryThumbContract from '@DataContracts/EntryThumbContract';
import EntryTypeAndSubTypeContract from '@DataContracts/EntryTypeAndSubTypeContract';
import EnglishTranslatedStringContract from '@DataContracts/Globalization/EnglishTranslatedStringContract';
import EventSeriesContract from '@DataContracts/ReleaseEvents/EventSeriesContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import SongContract from '@DataContracts/Song/SongContract';
import WebLinkContract from '@DataContracts/WebLinkContract';

import TagBaseContract from './TagBaseContract';

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
export default interface TagDetailsContract {
	additionalNames?: string;
	allUsageCount: number;
	categoryName?: string;
	children: TagBaseContract[];
	commentCount: number;
	createDate: Date;
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
	status: string;
	targets: number;
	translations: string;
	webLinks: WebLinkContract[];
}
