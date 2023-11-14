import { EntryContract } from "./entry"
import { UserApiContract } from "./user"

export interface CommentContract {
  author: UserApiContract
  authorName?: string
  canBeDeleted?: boolean
  canBeEdited?: boolean
  created: string
  entry?: EntryContract
  id?: number
  message: string
}
