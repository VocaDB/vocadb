import PartialImportedSongs from './PartialImportedSongs';

export default interface ImportedSongListContract {
	createDate: string;

	description: string;

	name: string;

	songs: PartialImportedSongs;

	wvrNumber: string;
}
