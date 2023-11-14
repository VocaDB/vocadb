import { EntryStatus } from "./entry"

export interface TagBaseContract {
  // Additional names list - optional field
  additionalNames?: string
  categoryName?: string
  id: number
  name: string
  status: EntryStatus
  urlSlug?: string
}
