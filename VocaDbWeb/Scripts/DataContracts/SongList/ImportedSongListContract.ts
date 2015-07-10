
module vdb.dataContracts.songList {
	
	export interface ImportedSongListContract {
		
		createDate: string;

		description: string;

		name: string;

		songs: PartialImportedSongs;

		wvrNumber: string;

	}

} 