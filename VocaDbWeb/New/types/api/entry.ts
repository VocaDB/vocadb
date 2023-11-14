import { AlbumType } from "./album"
import { ArtistType } from "./artist"
import { CommentContract } from "./comment"
import { SongType } from "./song"
import { TagUsageForApiContract } from "./tag-usage"

export interface IEntryWithIdAndName {
  id: number
  name?: string
}

// Note: matches C# class EntryThumbForApiContract
export interface EntryThumbContract {
  name?: string
  urlOriginal?: string
  urlSmallThumb?: string
  urlTinyThumb?: string
  urlThumb?: string
}

/**
 * @deprecated The method should not be used
 */
export interface EntryWithTagUsagesContract {
  id: number
  name: string
  tags?: TagUsageForApiContract[]
}

// Identifies common entry type.
// Corresponds to the EntryType enum C#.
export enum EntryType {
  Undefined = "Undefined",
  Album = "Album",
  Artist = "Artist",
  DiscussionTopic = "DiscussionTopic",
  PV = "PV",
  ReleaseEvent = "ReleaseEvent",
  ReleaseEventSeries = "ReleaseEventSeries",
  Song = "Song",
  SongList = "SongList",
  Tag = "Tag",
  User = "User",
  Venue = "Venue",
}

// Base data contract for entries from the API.
// Corresponds to C# datacontract EntryForApiContract.
export interface EntryContract extends EntryWithTagUsagesContract {
  additionalNames?: string
  artistString?: string
  artistType?: ArtistType
  discType?: AlbumType
  entryType: EntryType
  eventCategory?: string
  id: number
  mainPicture?: EntryThumbContract
  name: string
  releaseEventSeriesName?: string
  songListFeaturedCategory?: string
  songType?: SongType
  status: EntryStatus
  tagCategoryName?: string
  urlSlug?: string
}

export enum EntryStatus {
  Draft = "Draft",
  Finished = "Finished",
  Approved = "Approved",
  Locked = "Locked",
}

export enum EntryEditEvent {
  Created = "Created",
  Updated = "Updated",
}

export interface CommonEntryContract {
  createDate?: string
  id: number
  name: string
  status: EntryStatus
  version?: number
}

// Corresponds to the EntryWithCommentsContract class in C#.
export interface EntryWithCommentsContract {
  comments: CommentContract[]
  entry: EntryContract
}
