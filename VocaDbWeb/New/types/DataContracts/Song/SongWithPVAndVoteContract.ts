import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongContract } from '@/types/DataContracts/Song/SongContract';

// Corresponds to the SongWithPVAndVoteForApiContract class in C#.
export interface SongWithPVAndVoteContract extends SongContract {
	pvs: PVContract[];
	vote: string;
}
