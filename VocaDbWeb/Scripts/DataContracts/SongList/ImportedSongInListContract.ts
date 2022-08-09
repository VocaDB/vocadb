import { SongApiContract } from '@/DataContracts/Song/SongApiContract';

export interface ImportedSongInListContract {
	matchedSong: SongApiContract;

	name: string;

	pvId: string;

	pvService: string;

	sortIndex: number;

	url: string;
}
