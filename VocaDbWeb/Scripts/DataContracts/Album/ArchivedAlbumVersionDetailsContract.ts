import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { ArchivedAlbumContract } from '@/DataContracts/Album/ArchivedAlbumContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedAlbumVersionDetailsForApiContract record class in C#.
export interface ArchivedAlbumVersionDetailsContract {
	album: AlbumForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedAlbumContract>;
}
