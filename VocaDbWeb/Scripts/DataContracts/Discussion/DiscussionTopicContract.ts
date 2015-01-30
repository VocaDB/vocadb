
module vdb.dataContracts.discussions {
	
	export interface DiscussionTopicContract {
		
		author: user.UserApiContract;

		canBeDeleted: boolean;

		commentCount: number;

		comments: CommentContract[];

		content: string;

		createDate: Date;

		folderId: number;

		id: number;

		lastCommentDate: Date;

		name: string;

	}

} 