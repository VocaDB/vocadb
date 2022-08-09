import { EntryRefContract } from '@/DataContracts/EntryRefContract';

export interface IEntryWithStatus extends EntryRefContract {
	status: string;
}
