import SongWithPVAndVoteContract from './SongWithPVAndVoteContract';

export default interface SongWithPVPlayerAndVoteContract {
	playerHtml: string;

	pvService: string;

	song: SongWithPVAndVoteContract;
}
