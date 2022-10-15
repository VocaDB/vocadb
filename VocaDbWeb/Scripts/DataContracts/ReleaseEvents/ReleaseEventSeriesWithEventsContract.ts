import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';

// Corresponds to the ReleaseEventSeriesWithEventsForApiContract record class in C#.
export interface ReleaseEventSeriesWithEventsContract {
	description: string;
	events: ReleaseEventContract[];
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
}
