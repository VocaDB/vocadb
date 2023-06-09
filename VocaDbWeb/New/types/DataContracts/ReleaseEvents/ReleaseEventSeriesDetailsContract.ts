import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EventCategory } from '@/types/Models/Events/EventCategory';

// Corresponds to the ReleaseEventSeriesDetailsForApiContract record class in C#.
export interface ReleaseEventSeriesDetailsContract {
	additionalNames: string;
	category: EventCategory;
	deleted: boolean;
	description: string;
	events: ReleaseEventContract[];
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	status: EntryStatus;
	tags: TagUsageForApiContract[];
	webLinks: WebLinkContract[];
}
