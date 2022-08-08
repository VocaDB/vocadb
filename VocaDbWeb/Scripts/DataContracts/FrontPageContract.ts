import ActivityEntryContract from '@/DataContracts/ActivityEntry/ActivityEntryContract';
import AlbumForApiContract from '@/DataContracts/Album/AlbumForApiContract';
import EntryWithCommentsContract from '@/DataContracts/EntryWithCommentsContract';
import ReleaseEventContract from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import SongWithPVAndVoteContract from '@/DataContracts/Song/SongWithPVAndVoteContract';

// Corresponds to the FrontPageForApiContract record class in C#.
export default interface FrontPageContract {
	activityEntries: ActivityEntryContract[];
	firstSong: SongWithPVAndVoteContract;
	newAlbums: AlbumForApiContract[];
	newEvents: ReleaseEventContract[];
	newSongs: SongWithPVAndVoteContract[];
	recentComments: EntryWithCommentsContract[];
	topAlbums: AlbumForApiContract[];
}
