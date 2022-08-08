import PVContract from '@/DataContracts/PVs/PVContract';
import SongContract from '@/DataContracts/Song/SongContract';

// Corresponds to the SongWithPVAndVoteForApiContract class in C#.
export default interface SongWithPVAndVoteContract extends SongContract {
	pvs: PVContract[];
	vote: string;
}
