import ActivityEntryContract from './ActivityEntry/ActivityEntryContract';
import AlbumForApiContract from './Album/AlbumForApiContract';
import EntryWithCommentsContract from './EntryWithCommentsContract';
import ReleaseEventContract from './ReleaseEvents/ReleaseEventContract';
import SongWithPVAndVoteContract from './Song/SongWithPVAndVoteContract';

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
