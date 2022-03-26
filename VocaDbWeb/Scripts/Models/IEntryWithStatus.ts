import EntryRefContract from '@DataContracts/EntryRefContract';

export default interface IEntryWithStatus extends EntryRefContract {
	status: string;
}
