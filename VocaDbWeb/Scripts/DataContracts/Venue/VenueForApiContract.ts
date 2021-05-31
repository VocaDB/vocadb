import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

export default interface VenueForApiContract {
	id: number;

	name: string;

	names?: LocalizedStringWithIdContract[];

	webLinks: WebLinkContract[];
}
