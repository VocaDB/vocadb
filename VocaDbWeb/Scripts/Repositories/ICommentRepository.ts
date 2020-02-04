
import CommentContract from '../DataContracts/CommentContract';

//module vdb.repositories {
	
	// Repository for managing comments
	export default interface ICommentRepository {
		
		createComment(entryId: number, contract: CommentContract, callback: (contract: CommentContract) => void): void;

		deleteComment(commentId: number, callback?: () => void): void;

		getComments(entryId: number, callback: (contract: CommentContract[]) => void): void;

		updateComment(commentId: number, contract: CommentContract, callback?: () => void): void;

	}

//} 