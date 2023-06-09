import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { OptionalGeoPointContract } from '@/types/DataContracts/OptionalGeoPointContract';

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
