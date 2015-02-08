
module vdb.repositories {
	
	import dc = vdb.dataContracts;

	// Repository for managing comments
	export interface ICommentRepository {
		
		createComment(entryId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void);

		deleteComment(commentId: number, callback?: () => void);

		updateComment(commentId: number, contract: dc.CommentContract, callback?: () => void);

	}

} 