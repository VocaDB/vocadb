import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/types/DataContracts/ReleaseEvents/ArtistForEventContract';
import { ReleaseEventSeriesForApiContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { SongInListContract } from '@/types/DataContracts/Song/SongInListContract';
import { SongListBaseContract } from '@/types/DataContracts/SongListBaseContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';
import { VenueForApiContract } from '@/types/DataContracts/Venue/VenueForApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EventCategory } from '@/types/Models/Events/EventCategory';

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
