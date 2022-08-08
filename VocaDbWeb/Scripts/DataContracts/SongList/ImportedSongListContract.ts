import PartialImportedSongs from '@/DataContracts/SongList/PartialImportedSongs';

export default interface ImportedSongListContract {
	createDate: string;

	description: string;

	name: string;

	songs: PartialImportedSongs;

	wvrNumber: string;
}
