import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import CommentContract from '@DataContracts/CommentContract';
import EntryThumbContract from '@DataContracts/EntryThumbContract';
import PVContract from '@DataContracts/PVs/PVContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import SongInListContract from '@DataContracts/Song/SongInListContract';
import SongListBaseContract from '@DataContracts/SongListBaseContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import UserApiContract from '@DataContracts/User/UserApiContract';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import WebLinkContract from '@DataContracts/WebLinkContract';

import ArtistForEventContract from './ArtistForEventContract';
import ReleaseEventSeriesForApiContract from './ReleaseEventSeriesForApiContract';

// Corresponds to the ReleaseEventDetailsForApiContract record class in C#.
export default interface ReleaseEventDetailsContract {
	additionalNames: string;
	albums: AlbumForApiContract[];
	artists: ArtistForEventContract[];
	canRemoveTagUsages: boolean;
	date?: Date;
	deleted: boolean;
	description: string;
	endDate?: Date;
	eventAssociationType: string /* TODO: enum */;
	id: number;
	inheritedCategory: string /* TODO: enum */;
	inheritedCategoryTag?: TagBaseContract;
	latestComments: CommentContract[];
	mainPicture?: EntryThumbContract;
	name: string;
	pvs: PVContract[];
	series?: ReleaseEventSeriesForApiContract;
	songList?: SongListBaseContract;
	songListSongs?: SongInListContract[];
	songs: SongApiContract[];
	status: string /* TODO: enum */;
	tags: TagUsageForApiContract[];
	usersAttending: UserApiContract[];
	venue?: VenueForApiContract;
	venueName?: string;
	webLinks: WebLinkContract[];
}
