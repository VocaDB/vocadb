import CommentContract from '@DataContracts/CommentContract';

// Repository for managing comments
export default interface ICommentRepository {
	createComment({
		entryId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract>;

	deleteComment({ commentId }: { commentId: number }): Promise<void>;

	getComments({ entryId }: { entryId: number }): Promise<CommentContract[]>;

	updateComment({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void>;
}
