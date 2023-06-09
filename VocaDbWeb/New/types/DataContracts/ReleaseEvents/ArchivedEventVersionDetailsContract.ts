import { ArchivedEventContract } from '@/types/DataContracts/ReleaseEvents/ArchivedEventContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedEventVersionDetailsForApiContract record class in C#.
export interface ArchivedEventVersionDetailsContract {
	releaseEvent: ReleaseEventContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedEventContract>;
}
