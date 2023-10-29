import { ArtistForAlbumContract } from "./artist"
import {
  CommonEntryContract,
  EntryStatus,
  EntryThumbContract,
  EntryWithTagUsagesContract,
} from "./entry"
import { LocalizedStringContract } from "./localized"
import { PVContract, PVService } from "./pv"

export interface SongContract extends CommonEntryContract {
  additionalNames: string
  artistString: string
  favoritedTimes?: number
  lengthSeconds: number
  mainPicture?: EntryThumbContract
  // Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
  publishDate?: string
  pvs?: PVContract[]
  pvServices: string
  ratingScore: number
  songType: SongType
  status: EntryStatus
  thumbUrl?: string
  version: number
}

export enum SongType {
  Unspecified = "Unspecified",
  Original = "Original",
  Remaster = "Remaster",
  Remix = "Remix",
  Cover = "Cover",
  Arrangement = "Arrangement",
  Instrumental = "Instrumental",
  Mashup = "Mashup",
  MusicPV = "MusicPV",
  DramaPV = "DramaPV",
  Live = "Live",
  Illustration = "Illustration",
  Other = "Other",
}

// Corresponds to the SongWithPVAndVoteForApiContract class in C#.
export interface SongWithPVAndVoteContract extends SongContract {
  pvs: PVContract[]
  vote: string
}

// Corresponds to the SongInAlbumForApiContract in C#.
interface SongInAlbumContract {
  discNumber: number
  song: SongApiContract
  trackNumber: number
}

export interface SongApiContract
  extends SongContract,
    EntryWithTagUsagesContract {
  artists?: ArtistForAlbumContract[]
  defaultName?: string
  names?: LocalizedStringContract[]
  // Not returned from the API, but can be used to cache the list of PV services client side
  pvServicesArray?: PVService[]
  urlFriendlyName?: string
}
