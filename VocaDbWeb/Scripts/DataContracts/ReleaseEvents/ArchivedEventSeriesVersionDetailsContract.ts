import { ArchivedEventSeriesContract } from '@/DataContracts/ReleaseEvents/ArchivedEventSeriesContract';
import { ReleaseEventSeriesForApiContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedEventSeriesVersionDetailsForApiContract record class in C#.
export interface ArchivedEventSeriesVersionDetailsContract {
	releaseEventSeries: ReleaseEventSeriesForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedEventSeriesContract>;
}
