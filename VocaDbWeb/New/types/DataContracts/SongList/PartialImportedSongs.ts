import { ImportedSongInListContract } from '@/types/DataContracts/SongList/ImportedSongInListContract';

export interface PartialImportedSongs {
	items: ImportedSongInListContract[];

	nextPageToken: string;

	totalCount: number;
}
