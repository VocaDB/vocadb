import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import { PVService } from '@/Models/PVs/PVService';

export interface SongWithPVPlayerAndVoteContract {
	playerHtml: string;

	pvId: string;

	pvService: PVService;

	song: SongWithPVAndVoteContract;
}
