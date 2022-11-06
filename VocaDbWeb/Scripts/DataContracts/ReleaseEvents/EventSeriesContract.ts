import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EventCategory } from '@/Models/Events/EventCategory';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';

// Matches ReleaseEventForApiContract
export interface EventSeriesContract extends IEntryWithIdAndName {
	additionalNames?: string;

	category: EventCategory;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	names?: LocalizedStringWithIdContract[];

	urlSlug?: string;

	webLinks: WebLinkContract[];
}
