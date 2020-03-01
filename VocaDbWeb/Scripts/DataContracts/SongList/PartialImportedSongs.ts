import ImportedSongInListContract from './ImportedSongInListContract';

//module vdb.dataContracts.songList {
	
	export default interface PartialImportedSongs {

		items: ImportedSongInListContract[];

		nextPageToken: string;

		totalCount: number;
		
	}

//} 