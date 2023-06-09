import { ActivityEntryContract } from '@/types/DataContracts/ActivityEntry/ActivityEntryContract';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { EntryWithCommentsContract } from '@/types/DataContracts/EntryWithCommentsContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';

// Corresponds to the FrontPageForApiContract record class in C#.
export interface FrontPageContract {
	activityEntries: ActivityEntryContract[];
	firstSong: SongWithPVAndVoteContract;
	newAlbums: AlbumForApiContract[];
	newEvents: ReleaseEventContract[];
	newSongs: SongWithPVAndVoteContract[];
	recentComments: EntryWithCommentsContract[];
	topAlbums: AlbumForApiContract[];
}
