import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/types/DataContracts/ReleaseEvents/ArtistForEventContract';
import { EventSeriesContract } from '@/types/DataContracts/ReleaseEvents/EventSeriesContract';
import { SongListBaseContract } from '@/types/DataContracts/SongListBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { VenueForApiContract } from '@/types/DataContracts/Venue/VenueForApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EventCategory } from '@/types/Models/Events/EventCategory';

// Matches ReleaseEventForApiContract
export interface ReleaseEventContract {
	additionalNames?: string;
	artists: ArtistForEventContract[];
	category: EventCategory;
	date?: string;
	defaultNameLanguage: string;
	description?: string;
	endDate?: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names?: LocalizedStringWithIdContract[];
	pvs?: PVContract[];
	series?: EventSeriesContract;
	songList?: SongListBaseContract;
	status: EntryStatus;
	tags?: TagUsageForApiContract[];
	urlSlug?: string;
	venue?: VenueForApiContract;
	version?: number;
	venueName?: string;
	webLinks: WebLinkContract[];
}
