import { PartialImportedSongs } from '@/DataContracts/SongList/PartialImportedSongs';

export interface ImportedSongListContract {
	createDate: string;

	description: string;

	name: string;

	songs: PartialImportedSongs;

	wvrNumber: string;
}
