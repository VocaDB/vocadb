import { ArchivedTagContract } from '@/types/DataContracts/Tag/ArchivedTagContract';
import { TagApiContract } from '@/types/DataContracts/Tag/TagApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedTagVersionDetailsForApiContract record class in C#.
export interface ArchivedTagVersionDetailsContract {
	tag: TagApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedTagContract>;
}
