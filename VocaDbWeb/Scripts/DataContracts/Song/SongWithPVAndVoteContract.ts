
module vdb.dataContracts.songs {

	export interface SongWithPVAndVoteContract extends SongContract {

		vote: string;

	}

}