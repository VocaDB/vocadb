import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';

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
