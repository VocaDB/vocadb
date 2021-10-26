import EntryRefContract from './EntryRefContract';

// Matches .NET class EntryBaseContract.
export default interface EntryBaseContract extends EntryRefContract {
	defaultName: string;
}
