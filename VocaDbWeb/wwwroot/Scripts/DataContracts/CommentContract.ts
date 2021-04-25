import EntryContract from './EntryContract';
import UserApiContract from './User/UserApiContract';

export default interface CommentContract {
  author: UserApiContract;

  authorName?: string;

  canBeDeleted?: boolean;

  canBeEdited?: boolean;

  created?: Date;

  entry?: EntryContract;

  id?: number;

  message: string;
}
