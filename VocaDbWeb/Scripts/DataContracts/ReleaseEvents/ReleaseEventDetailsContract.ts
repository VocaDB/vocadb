import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { ReleaseEventSeriesForApiContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongInListContract } from '@/DataContracts/Song/SongInListContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EventCategory } from '@/Models/Events/EventCategory';

// Corresponds to the ReleaseEventDetailsForApiContract record class in C#.
export interface ReleaseEventDetailsContract {
	additionalNames: string;
	albums: AlbumForApiContract[];
	artists: ArtistForEventContract[];
	canRemoveTagUsages: boolean;
	date?: string;
	deleted: boolean;
	description: string;
	endDate?: string;
	eventAssociationType: string /* TODO: enum */;
	id: number;
	inheritedCategory: EventCategory;
	inheritedCategoryTag?: TagBaseContract;
	commentsLocked: boolean;
	latestComments: CommentContract[];
	mainPicture?: EntryThumbContract;
	name: string;
	pvs: PVContract[];
	series?: ReleaseEventSeriesForApiContract;
	songList?: SongListBaseContract;
	songListSongs?: SongInListContract[];
	songs: SongApiContract[];
	status: EntryStatus;
	tags: TagUsageForApiContract[];
	usersAttending: UserApiContract[];
	venue?: VenueForApiContract;
	venueName?: string;
	webLinks: WebLinkContract[];
}
