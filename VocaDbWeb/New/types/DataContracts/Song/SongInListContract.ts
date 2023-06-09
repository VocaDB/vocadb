import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';

export interface SongInListContract {
	order: number;

	notes: string;

	song: SongApiContract;
}
