import ImportedSongInListContract from '@/DataContracts/SongList/ImportedSongInListContract';

export default interface PartialImportedSongs {
	items: ImportedSongInListContract[];

	nextPageToken: string;

	totalCount: number;
}
