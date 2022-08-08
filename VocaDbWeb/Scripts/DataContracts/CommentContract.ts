import EntryContract from '@/DataContracts/EntryContract';
import UserApiContract from '@/DataContracts/User/UserApiContract';

export default interface CommentContract {
	author: UserApiContract;

	authorName?: string;

	canBeDeleted?: boolean;

	canBeEdited?: boolean;

	created?: Date;

	entry?: EntryContract;

	id?: number;

	message: string;
}
