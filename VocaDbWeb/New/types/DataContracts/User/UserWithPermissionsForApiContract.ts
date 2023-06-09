import { ArtistForUserForApiContract } from '@/types/DataContracts/User/ArtistForUserForApiContract';
import { PermissionToken } from '@/types/Models/LoginManager';
import { UserGroup } from '@/types/Models/Users/UserGroup';

// Corresponds to the ServerOnlyUserWithPermissionsForApiContract record class in C#.
export interface UserWithPermissionsForApiContract {
	active: boolean;
	additionalPermissions: PermissionToken[];
	effectivePermissions: PermissionToken[];
	email: string;
	groupId: UserGroup;
	id: number;
	name: string;
	ownedArtistEntries: ArtistForUserForApiContract[];
	poisoned: boolean;
	supporter: boolean;
}
