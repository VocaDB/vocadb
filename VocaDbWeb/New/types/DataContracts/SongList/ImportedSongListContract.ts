import { PartialImportedSongs } from '@/types/DataContracts/SongList/PartialImportedSongs';

export interface ImportedSongListContract {
	createDate: string;

	description: string;

	name: string;

	songs: PartialImportedSongs;

	wvrNumber: string;
}
