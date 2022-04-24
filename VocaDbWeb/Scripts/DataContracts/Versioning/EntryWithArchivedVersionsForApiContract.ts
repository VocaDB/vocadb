import ArchivedVersionContract from './ArchivedVersionContract';

// C# class: EntryWithArchivedVersionsForApiContract
export default interface EntryWithArchivedVersionsContract<TEntry> {
	entry: TEntry;
	archivedVersions: ArchivedVersionContract[];
}
