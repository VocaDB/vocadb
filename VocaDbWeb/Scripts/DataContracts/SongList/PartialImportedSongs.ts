import ImportedSongInListContract from './ImportedSongInListContract';

export default interface PartialImportedSongs {
	items: ImportedSongInListContract[];

	nextPageToken: string;

	totalCount: number;
}
