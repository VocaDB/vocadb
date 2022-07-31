import { ArchivedEventContract } from '@/DataContracts/ReleaseEvents/ArchivedEventContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedEventVersionDetailsForApiContract record class in C#.
export interface ArchivedEventVersionDetailsContract {
	releaseEvent: ReleaseEventContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedEventContract>;
}
