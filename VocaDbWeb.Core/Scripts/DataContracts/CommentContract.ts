import EntryRefContract from './EntryRefContract';
import UserApiContract from './User/UserApiContract';

	export default interface CommentContract {

		author: UserApiContract;

		authorName?: string;

		canBeDeleted?: boolean;

		canBeEdited?: boolean;

		created?: Date;

		entry?: EntryRefContract;

		id?: number;

		message: string;

	}