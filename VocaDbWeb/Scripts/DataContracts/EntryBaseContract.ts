import { EntryRefContract } from '@/DataContracts/EntryRefContract';

// Matches .NET class EntryBaseContract.
export interface EntryBaseContract extends EntryRefContract {
	defaultName?: string;
}
