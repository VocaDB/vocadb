import SongApiContract from '@/DataContracts/Song/SongApiContract';

export default interface SongInListContract {
	order: number;

	notes: string;

	song: SongApiContract;
}
