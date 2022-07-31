import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { OptionalGeoPointContract } from '@/DataContracts/OptionalGeoPointContract';

export interface ArchivedVenueContract {
	address: string;
	addressCountryCode: string;
	coordinates: OptionalGeoPointContract;
	description: string;
	id: number;
	names?: LocalizedStringContract[];
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
