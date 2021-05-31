import CommentContract from '@DataContracts/CommentContract';

// Repository for managing comments
export default interface ICommentRepository {
	createComment(
		entryId: number,
		contract: CommentContract,
	): Promise<CommentContract>;

	deleteComment(commentId: number): Promise<void>;

	getComments(entryId: number): Promise<CommentContract[]>;

	updateComment(commentId: number, contract: CommentContract): Promise<void>;
}
