import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';

// Corresponds to the ReleaseEventSeriesWithEventsForApiContract record class in C#.
export interface ReleaseEventSeriesWithEventsContract {
	description: string;
	events: ReleaseEventContract[];
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
}
