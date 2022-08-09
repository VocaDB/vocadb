import { CommentContract } from '@/DataContracts/CommentContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';

export interface DiscussionTopicContract {
	author: UserApiContract;

	canBeDeleted?: boolean;

	canBeEdited?: boolean;

	commentCount: number;

	comments: CommentContract[];

	content: string;

	created: Date;

	folderId: number;

	id: number;

	lastComment?: CommentContract;

	lastCommentDate: Date;

	locked: boolean;

	name: string;
}
