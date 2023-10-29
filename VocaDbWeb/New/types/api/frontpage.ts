import { ActivityEntryContract } from "./activity-entry"
import { AlbumForApiContract } from "./album"
import { EntryWithCommentsContract } from "./entry"
import { ReleaseEventContract } from "./release-event"
import { SongWithPVAndVoteContract } from "./song"

export interface FrontPageContract {
  activityEntries: ActivityEntryContract[]
  firstSong: SongWithPVAndVoteContract
  newAlbums: AlbumForApiContract[]
  newEvents: ReleaseEventContract[]
  newSongs: SongWithPVAndVoteContract[]
  recentComments: EntryWithCommentsContract[]
  topAlbums: AlbumForApiContract[]
}
