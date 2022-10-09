import { ArchivedTagContract } from '@/DataContracts/Tag/ArchivedTagContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedTagVersionDetailsForApiContract record class in C#.
export interface ArchivedTagVersionDetailsContract {
	tag: TagApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedTagContract>;
}
