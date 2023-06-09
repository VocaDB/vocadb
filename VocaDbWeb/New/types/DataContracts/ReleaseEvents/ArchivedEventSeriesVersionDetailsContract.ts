import { ArchivedEventSeriesContract } from '@/types/DataContracts/ReleaseEvents/ArchivedEventSeriesContract';
import { ReleaseEventSeriesForApiContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedEventSeriesVersionDetailsForApiContract record class in C#.
export interface ArchivedEventSeriesVersionDetailsContract {
	releaseEventSeries: ReleaseEventSeriesForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedEventSeriesContract>;
}
