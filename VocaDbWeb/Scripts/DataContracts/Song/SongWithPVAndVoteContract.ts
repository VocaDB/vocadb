import PVContract from '../PVs/PVContract';
import SongContract from './SongContract';

// Corresponds to the SongWithPVAndVoteForApiContract class in C#.
export default interface SongWithPVAndVoteContract extends SongContract {
	pvs: PVContract[];
	vote: string;
}
