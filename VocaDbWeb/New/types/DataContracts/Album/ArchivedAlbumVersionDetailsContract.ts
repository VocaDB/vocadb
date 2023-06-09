import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { ArchivedAlbumContract } from '@/types/DataContracts/Album/ArchivedAlbumContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedAlbumVersionDetailsForApiContract record class in C#.
export interface ArchivedAlbumVersionDetailsContract {
	album: AlbumForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedAlbumContract>;
}
