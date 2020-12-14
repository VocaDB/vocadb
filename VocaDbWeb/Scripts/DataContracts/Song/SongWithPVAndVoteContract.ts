import SongContract from './SongContract';

	export default interface SongWithPVAndVoteContract extends SongContract {

		vote: string;

	}