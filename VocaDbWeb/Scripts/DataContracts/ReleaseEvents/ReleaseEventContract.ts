import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';

import EntryThumbContract from '../EntryThumbContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongListBaseContract from '../SongListBaseContract';
import VenueForApiContract from '../Venue/VenueForApiContract';
import WebLinkContract from '../WebLinkContract';
import ArtistForEventContract from './ArtistForEventContract';
import EventSeriesContract from './EventSeriesContract';

// Matches ReleaseEventForApiContract
export default interface ReleaseEventContract {
	additionalNames?: string;

	artists: ArtistForEventContract[];

	category: string;

	date?: string;

	defaultNameLanguage: string;

	endDate?: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	pvs?: PVContract[];

	series?: EventSeriesContract;

	songList?: SongListBaseContract;

	status?: string;

	tags?: TagUsageForApiContract[];

	urlSlug?: string;

	venue?: VenueForApiContract;

	venueName?: string;

	webLinks: WebLinkContract[];
}
