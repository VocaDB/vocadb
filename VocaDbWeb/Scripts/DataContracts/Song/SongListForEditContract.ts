
import SongListContract from './SongListContract';
import SongInListEditContract from './SongInListEditContract';

//module vdb.dataContracts.songs {
	
	export default interface SongListForEditContract extends SongListContract {

		songLinks: SongInListEditContract[];

	}

//}