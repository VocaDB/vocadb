import { EntryRefContract } from '@/types/DataContracts/EntryRefContract';
import { EntryStatus } from '@/types/Models/EntryStatus';

export interface IEntryWithStatus extends EntryRefContract {
	status: EntryStatus;
}
