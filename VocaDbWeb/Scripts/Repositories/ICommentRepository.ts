
module vdb.repositories {
	
	import dc = vdb.dataContracts;

	// Repository for managing comments
	export interface ICommentRepository {
		
		createComment(entryId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void): void;

		deleteComment(commentId: number, callback?: () => void): void;

		getComments(entryId: number, callback: (contract: dc.CommentContract[]) => void): void;

		updateComment(commentId: number, contract: dc.CommentContract, callback?: () => void): void;

	}

} 