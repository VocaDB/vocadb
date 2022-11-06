import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { EntryStatus } from '@/Models/EntryStatus';

export interface IEntryWithStatus extends EntryRefContract {
	status: EntryStatus;
}
