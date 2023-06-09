import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EventCategory } from '@/types/Models/Events/EventCategory';
import { IEntryWithIdAndName } from '@/types/Models/IEntryWithIdAndName';

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
