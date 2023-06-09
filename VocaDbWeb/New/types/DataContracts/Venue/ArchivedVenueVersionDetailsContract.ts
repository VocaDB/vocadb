import { ArchivedVenueContract } from '@/types/DataContracts/Venue/ArchivedVenueContract';
import { VenueForApiContract } from '@/types/DataContracts/Venue/VenueForApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/types/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedVenueVersionDetailsForApiContract record class in C#.
export interface ArchivedVenueVersionDetailsContract {
	venue: VenueForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedVenueContract>;
}
