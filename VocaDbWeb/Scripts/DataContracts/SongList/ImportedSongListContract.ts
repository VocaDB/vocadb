import PartialImportedSongs from './PartialImportedSongs';

//module vdb.dataContracts.songList {
	
	export default interface ImportedSongListContract {
		
		createDate: string;

		description: string;

		name: string;

		songs: PartialImportedSongs;

		wvrNumber: string;

	}

//} 