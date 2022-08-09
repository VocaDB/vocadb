import { SongApiContract } from '@/DataContracts/Song/SongApiContract';

export interface SongInListContract {
	order: number;

	notes: string;

	song: SongApiContract;
}
