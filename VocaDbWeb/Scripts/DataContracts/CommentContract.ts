import { EntryContract } from '@/DataContracts/EntryContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';

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
