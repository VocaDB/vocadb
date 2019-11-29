
//module vdb.dataContracts.songList {
	
	export interface ImportedSongInListContract {

		matchedSong: SongApiContract;

		name: string;

		pvId: string;

		pvService: string;

		sortIndex: number;

		url: string;
		
	}

//}