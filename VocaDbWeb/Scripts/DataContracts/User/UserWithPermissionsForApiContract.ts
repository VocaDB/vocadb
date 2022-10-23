import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { PermissionToken } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';

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
