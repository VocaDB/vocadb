import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { UserBaseContract } from '@/types/DataContracts/User/UserBaseContract';
import { UserGroup } from '@/types/Models/Users/UserGroup';

export interface UserApiContract extends UserBaseContract {
	active?: boolean;
	groupId?: UserGroup;
	mainPicture?: EntryThumbContract;
	memberSince?: string;
	verifiedArtist?: boolean;
}
