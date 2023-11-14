import { EntryContract, EntryEditEvent } from "./entry"
import { UserApiContract } from "./user"
import { ArchivedVersionContract } from "./versioning"

export interface ActivityEntryContract {
  archivedVersion?: ArchivedVersionContract
  author?: UserApiContract
  createDate: string
  editEvent: EntryEditEvent
  entry: EntryContract
}
