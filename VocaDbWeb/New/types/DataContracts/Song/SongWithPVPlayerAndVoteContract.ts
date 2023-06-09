import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { PVService } from '@/types/Models/PVs/PVService';

export interface SongWithPVPlayerAndVoteContract {
	playerHtml: string;

	pvId: string;

	pvService: PVService;

	song: SongWithPVAndVoteContract;
}
