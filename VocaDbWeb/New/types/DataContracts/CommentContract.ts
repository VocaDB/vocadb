import { EntryContract } from '@/types/DataContracts/EntryContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';

export interface CommentContract {
	author: UserApiContract;
	authorName?: string;
	canBeDeleted?: boolean;
	canBeEdited?: boolean;
	created: string;
	entry?: EntryContract;
	id?: number;
	message: string;
}
