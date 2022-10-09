import { ArchivedVenueContract } from '@/DataContracts/Venue/ArchivedVenueContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';

// Corresponds to the ArchivedVenueVersionDetailsForApiContract record class in C#.
export interface ArchivedVenueVersionDetailsContract {
	venue: VenueForApiContract;
	archivedVersion: ArchivedVersionContract;
	comparableVersions: ArchivedVersionContract[];
	comparedVersion?: ArchivedVersionContract;
	name: string;
	versions: ComparedVersionsContract<ArchivedVenueContract>;
}
