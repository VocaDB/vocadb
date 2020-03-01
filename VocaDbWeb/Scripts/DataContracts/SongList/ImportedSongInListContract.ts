import SongApiContract from '../Song/SongApiContract';

//module vdb.dataContracts.songList {
	
	export default interface ImportedSongInListContract {

		matchedSong: SongApiContract;

		name: string;

		pvId: string;

		pvService: string;

		sortIndex: number;

		url: string;
		
	}

//}