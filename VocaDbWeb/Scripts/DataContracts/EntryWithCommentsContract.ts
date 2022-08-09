import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryContract } from '@/DataContracts/EntryContract';

// Corresponds to the EntryWithCommentsContract class in C#.
export interface EntryWithCommentsContract {
	comments: CommentContract[];
	entry: EntryContract;
}
