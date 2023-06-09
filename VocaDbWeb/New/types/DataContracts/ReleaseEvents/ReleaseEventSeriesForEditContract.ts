import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EventCategory } from '@/types/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/types/Models/Globalization/ContentLanguageSelection';

// Corresponds to the ReleaseEventSeriesForEditForApiContract record class in C#.
export interface ReleaseEventSeriesForEditContract {
	category: EventCategory;
	defaultNameLanguage: ContentLanguageSelection;
	deleted: boolean;
	description: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	names: LocalizedStringWithIdContract[];
	status: EntryStatus;
	webLinks: WebLinkContract[];
}
