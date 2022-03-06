import CommentContract from './CommentContract';
import EntryContract from './EntryContract';

// Corresponds to the EntryWithCommentsContract class in C#.
export default interface EntryWithCommentsContract {
	comments: CommentContract[];
	entry: EntryContract;
}
