import { EntryThumbContract, IEntryWithIdAndName } from "./entry"

// Corresponds to the UserGroupId enum in C#.
export enum UserGroup {
  Nothing = "Nothing",
  Limited = "Limited",
  Regular = "Regular",
  Trusted = "Trusted",
  Moderator = "Moderator",
  Admin = "Admin",
}

export interface UserBaseContract extends IEntryWithIdAndName {
  id: number
  name?: string
}

export interface UserApiContract extends UserBaseContract {
  active?: boolean
  groupId?: UserGroup
  mainPicture?: EntryThumbContract
  memberSince?: string
  verifiedArtist?: boolean
}
