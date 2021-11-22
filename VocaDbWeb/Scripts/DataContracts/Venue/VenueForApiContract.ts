import OptionalGeoPointContract from '@DataContracts/OptionalGeoPointContract';

import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

export default interface VenueForApiContract {
	additionalNames?: string;

	coordinates: OptionalGeoPointContract;

	description: string;

	id: number;

	name: string;

	names?: LocalizedStringWithIdContract[];

	webLinks: WebLinkContract[];
}
