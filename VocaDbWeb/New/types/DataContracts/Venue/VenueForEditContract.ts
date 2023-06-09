import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { OptionalGeoPointContract } from '@/types/DataContracts/OptionalGeoPointContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { ContentLanguageSelection } from '@/types/Models/Globalization/ContentLanguageSelection';

// Corresponds to the VenueForEditForApiContract record class in C#.
export interface VenueForEditContract {
	address: string;
	addressCountryCode: string;
	coordinates?: OptionalGeoPointContract;
	defaultNameLanguage: ContentLanguageSelection;
	deleted: boolean;
	description: string;
	id: number;
	name: string;
	names: LocalizedStringWithIdContract[];
	status: EntryStatus;
	webLinks: WebLinkContract[];
}
