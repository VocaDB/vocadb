
module vdb.dataContracts.discussions {
	
	export interface DiscussionTopicContract {
		
		author: user.UserApiContract;

		canBeDeleted?: boolean;

		canBeEdited?: boolean;

		commentCount: number;

		comments: CommentContract[];

		content: string;

		created: Date;

		folderId: number;

		id: number;

		lastCommentDate: Date;

		name: string;

	}

} 