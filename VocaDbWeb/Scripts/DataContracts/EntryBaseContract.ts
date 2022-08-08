import EntryRefContract from '@/DataContracts/EntryRefContract';

// Matches .NET class EntryBaseContract.
export default interface EntryBaseContract extends EntryRefContract {
	defaultName?: string;
}
