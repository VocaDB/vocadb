import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { UserGroup } from '@/Models/Users/UserGroup';

export interface UserApiContract extends UserBaseContract {
	active?: boolean;

	groupId?: UserGroup;

	mainPicture?: EntryThumbContract;

	memberSince?: Date;
}
