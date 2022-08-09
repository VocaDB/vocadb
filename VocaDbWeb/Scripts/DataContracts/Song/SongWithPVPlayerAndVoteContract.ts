import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';

export interface SongWithPVPlayerAndVoteContract {
	playerHtml: string;

	pvId: string;

	pvService: string;

	song: SongWithPVAndVoteContract;
}
