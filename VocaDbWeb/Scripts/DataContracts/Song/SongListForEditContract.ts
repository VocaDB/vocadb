
module vdb.dataContracts.songs {
	
	export interface SongListForEditContract extends SongListContract {

		songLinks: SongInListEditContract[];

	}

}