import { ArtistForAlbumContract } from "./artist"
import { OptionalDateTimeContract } from "./date-time"
import {
  CommonEntryContract,
  EntryStatus,
  EntryThumbContract,
  EntryWithTagUsagesContract,
} from "./entry"
import { PVContract } from "./pv"
import { ReleaseEventContract } from "./release-event"

export interface AlbumContract
  extends CommonEntryContract,
    EntryWithTagUsagesContract {
  additionalNames: string
  artistString: string
  discType: AlbumType
  mainPicture: EntryThumbContract
  ratingAverage: number
  ratingCount: number
  releaseDate: OptionalDateTimeContract
  releaseEvent?: ReleaseEventContract
  status: EntryStatus
  version: number
}

export interface AlbumForApiContract extends AlbumContract {
  artists?: ArtistForAlbumContract[]
  pvs?: PVContract[]
  tracks?: AlbumContract[]
}

export enum AlbumType {
  Unknown = "Unknown",
  Album = "Album",
  Single = "Single",
  EP = "EP",
  SplitAlbum = "SplitAlbum",
  Compilation = "Compilation",
  Video = "Video",
  Artbook = "Artbook",
  Game = "Game",
  Fanmade = "Fanmade",
  Instrumental = "Instrumental",
  Other = "Other",
}
