import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';

export interface ReleaseEventSeriesForApiContract {
	additionalNames: string;
	category: string;
	description: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names?: LocalizedStringWithIdContract[];
	status: EntryStatus;
	urlSlug: string;
	version?: number;
	webLinks: WebLinkContract[];
}
