import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';

// Matches ReleaseEventForApiContract
export interface EventSeriesContract extends IEntryWithIdAndName {
	additionalNames?: string;

	category: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	urlSlug?: string;

	webLinks: WebLinkContract[];
}
