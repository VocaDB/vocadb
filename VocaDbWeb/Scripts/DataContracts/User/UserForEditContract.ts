import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { PermissionToken } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';

export interface UserForEditContract {
	active: boolean;
	additionalPermissions: PermissionToken[];
	email: string;
	groupId: UserGroup;
	id: number;
	name: string;
	ownedArtistEntries: ArtistForUserForApiContract[];
	poisoned: boolean;
	supporter: boolean;
}
