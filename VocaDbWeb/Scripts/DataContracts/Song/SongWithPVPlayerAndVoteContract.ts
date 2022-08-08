import SongWithPVAndVoteContract from '@/DataContracts/Song/SongWithPVAndVoteContract';

export default interface SongWithPVPlayerAndVoteContract {
	playerHtml: string;

	pvId: string;

	pvService: string;

	song: SongWithPVAndVoteContract;
}
