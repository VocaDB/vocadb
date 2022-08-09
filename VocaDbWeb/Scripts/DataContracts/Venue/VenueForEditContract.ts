import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { OptionalGeoPointContract } from '@/DataContracts/OptionalGeoPointContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';

// Corresponds to the VenueForEditForApiContract record class in C#.
export interface VenueForEditContract {
	address: string;
	addressCountryCode: string;
	coordinates?: OptionalGeoPointContract;
	defaultNameLanguage: string;
	deleted: boolean;
	description: string;
	id: number;
	name: string;
	names: LocalizedStringWithIdContract[];
	status: string /* TODO: enum */;
	webLinks: WebLinkContract[];
}
