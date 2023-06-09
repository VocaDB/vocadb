import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { OptionalGeoPointContract } from '@/types/DataContracts/OptionalGeoPointContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';

export interface VenueForApiContract {
	additionalNames?: string;
	address: string;
	addressCountryCode: string;
	coordinates: OptionalGeoPointContract;
	deleted: boolean;
	description: string;
	events: ReleaseEventContract[];
	id: number;
	name: string;
	names?: LocalizedStringWithIdContract[];
	status: EntryStatus;
	version?: number;
	webLinks: WebLinkContract[];
}
