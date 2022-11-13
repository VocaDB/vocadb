import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { EventSeriesContract } from '@/DataContracts/ReleaseEvents/EventSeriesContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EventCategory } from '@/Models/Events/EventCategory';

// Matches ReleaseEventForApiContract
export interface ReleaseEventContract {
	additionalNames?: string;
	artists: ArtistForEventContract[];
	category: EventCategory;
	/**  DateOnly */
	date?: string;
	defaultNameLanguage: string;
	description?: string;
	/**  DateOnly */
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
