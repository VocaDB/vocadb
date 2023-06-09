import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryContract } from '@/types/DataContracts/EntryContract';

// Corresponds to the EntryWithCommentsContract class in C#.
export interface EntryWithCommentsContract {
	comments: CommentContract[];
	entry: EntryContract;
}
