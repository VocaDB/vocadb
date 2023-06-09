import { CommentContract } from '@/types/DataContracts/CommentContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';

export interface DiscussionTopicContract {
	author: UserApiContract;
	canBeDeleted?: boolean;
	canBeEdited?: boolean;
	commentCount: number;
	comments: CommentContract[];
	content: string;
	created: string;
	folderId: number;
	id: number;
	lastComment?: CommentContract;
	lastCommentDate: string;
	locked: boolean;
	name: string;
}
