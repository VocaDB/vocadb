import { EntryRefContract } from '@/types/DataContracts/EntryRefContract';

// Matches .NET class EntryBaseContract.
export interface EntryBaseContract extends EntryRefContract {
	defaultName?: string;
}
