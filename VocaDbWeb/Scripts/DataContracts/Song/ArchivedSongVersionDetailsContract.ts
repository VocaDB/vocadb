import { ArchivedSongContract } from '@/DataContracts/Song/ArchivedSongContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedSongVersionDetailsForApiContract record class in C#.
export interface ArchivedSongVersionDetailsContract {
	song: SongApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedSongContract>;
}
