import { ArchivedSongContract } from '@/types/DataContracts/Song/ArchivedSongContract';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedSongVersionDetailsForApiContract record class in C#.
export interface ArchivedSongVersionDetailsContract {
	song: SongApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedSongContract>;
}
