import ArchivedVersionContract from '@/DataContracts/Versioning/ArchivedVersionContract';

// C# class: EntryWithArchivedVersionsForApiContract
export default interface EntryWithArchivedVersionsContract<TEntry> {
	entry: TEntry;
	archivedVersions: ArchivedVersionContract[];
}
