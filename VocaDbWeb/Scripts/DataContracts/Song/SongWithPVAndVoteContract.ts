import SongContract from './SongContract';

//module vdb.dataContracts.songs {

	export default interface SongWithPVAndVoteContract extends SongContract {

		vote: string;

	}

//}