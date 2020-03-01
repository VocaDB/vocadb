import SongApiContract from './SongApiContract';

//module vdb.dataContracts.songs {
	
	export default interface SongInListContract {

		order: number;

		notes: string;

		song: SongApiContract;

	}

//}