import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import OptionalGeoPointContract from '../OptionalGeoPointContract';
import WebLinkContract from '../WebLinkContract';

export default interface VenueForEditContract {
  address: string;

  addressCountryCode: string;

  coordinates: OptionalGeoPointContract;

  defaultNameLanguage: string;

  id: number;

  names?: LocalizedStringWithIdContract[];

  webLinks: WebLinkContract[];
}
