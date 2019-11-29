
//module vdb.dataContracts.songList {
	
	export interface PartialImportedSongs {

		items: ImportedSongInListContract[];

		nextPageToken: string;

		totalCount: number;
		
	}

//} 