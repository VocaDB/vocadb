import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import OptionalGeoPointContract from '../OptionalGeoPointContract';
import WebLinkContract from '../WebLinkContract';

// Corresponds to the VenueForEditForApiContract record class in C#.
export default interface VenueForEditContract {
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
