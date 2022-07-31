import { ArchivedArtistContract } from '@/DataContracts/Artist/ArchivedArtistContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedArtistVersionDetailsForApiContract record class in C#.
export interface ArchivedArtistVersionDetailsContract {
	artist: ArtistApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedArtistContract>;
}
