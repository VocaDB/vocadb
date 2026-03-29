import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';

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
	updateNotes: string;
	webLinks: WebLinkContract[];
}
