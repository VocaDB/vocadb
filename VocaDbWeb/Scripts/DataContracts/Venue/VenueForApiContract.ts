import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { OptionalGeoPointContract } from '@/DataContracts/OptionalGeoPointContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';

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

	status: string /* TODO: enum */;

	version?: number;

	webLinks: WebLinkContract[];
}
