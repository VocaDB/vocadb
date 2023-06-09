import { EntryType } from '@/types/Models/EntryType';

// Matches .NET class EntryRefContract.
export interface EntryRefContract {
	entryType: EntryType;
	id: number;
}
