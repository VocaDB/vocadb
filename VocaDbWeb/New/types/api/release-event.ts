import { ArtistContract } from "./artist"
import { EntryStatus, EntryThumbContract, IEntryWithIdAndName } from "./entry"
import { LocalizedStringWithIdContract } from "./localized"
import { PVContract } from "./pv"
import { SongListBaseContract } from "./songlist"
import { TagUsageForApiContract } from "./tag-usage"
import { VenueForApiContract } from "./venue"
import { WebLinkContract } from "./weblink"

// Matches ReleaseEventForApiContract
export interface ReleaseEventContract {
  additionalNames?: string
  artists: ArtistForEventContract[]
  category: EventCategory
  date?: string
  defaultNameLanguage: string
  description?: string
  endDate?: string
  id: number
  mainPicture?: EntryThumbContract
  name: string
  names?: LocalizedStringWithIdContract[]
  pvs?: PVContract[]
  series?: EventSeriesContract
  songList?: SongListBaseContract
  status: EntryStatus
  tags?: TagUsageForApiContract[]
  urlSlug?: string
  venue?: VenueForApiContract
  version?: number
  venueName?: string
  webLinks: WebLinkContract[]
}

export interface ArtistForEventContract {
  artist?: ArtistContract
  effectiveRoles?: string
  id?: number
  name?: string
  roles: string
}

// Corresponds to the EventCategory enum in C#.
export enum EventCategory {
  Unspecified = "Unspecified",
  AlbumRelease = "AlbumRelease",
  Anniversary = "Anniversary",
  Club = "Club",
  Concert = "Concert",
  Contest = "Contest",
  Convention = "Convention",
  Other = "Other",
  Festival = "Festival",
}

export interface EventSeriesContract extends IEntryWithIdAndName {
  additionalNames?: string
  category: EventCategory
  id: number
  mainPicture?: EntryThumbContract
  name: string
  names?: LocalizedStringWithIdContract[]
  urlSlug?: string
  webLinks: WebLinkContract[]
}
