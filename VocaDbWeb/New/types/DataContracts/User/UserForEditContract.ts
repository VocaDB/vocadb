import { ArtistForUserForApiContract } from '@/types/DataContracts/User/ArtistForUserForApiContract';
import { PermissionToken } from '@/types/Models/LoginManager';
import { UserGroup } from '@/types/Models/Users/UserGroup';

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
