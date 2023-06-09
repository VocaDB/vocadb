import { ArchivedArtistContract } from '@/types/DataContracts/Artist/ArchivedArtistContract';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedArtistVersionDetailsForApiContract record class in C#.
export interface ArchivedArtistVersionDetailsContract {
	artist: ArtistApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedArtistContract>;
}
