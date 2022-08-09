import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';

// C# class: EntryWithArchivedVersionsForApiContract
export interface EntryWithArchivedVersionsContract<TEntry> {
	entry: TEntry;
	archivedVersions: ArchivedVersionContract[];
}
