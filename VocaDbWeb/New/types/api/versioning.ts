import { UserApiContract } from "./user"

// C# class: ArchivedObjectVersionForApiContract
export interface ArchivedVersionContract {
  agentName: string
  anythingChanged: boolean
  author?: UserApiContract
  changedFields: string[]
  created: string
  hidden: boolean
  id: number
  isSnapshot: boolean
  notes: string
  reason: string
  status: string
  version: number
}
