import OptionalGeoPointContract from '@DataContracts/OptionalGeoPointContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';

import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

export default interface VenueForApiContract {
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

	webLinks: WebLinkContract[];
}
