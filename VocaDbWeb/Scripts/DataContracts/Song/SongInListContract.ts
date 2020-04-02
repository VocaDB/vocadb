import SongApiContract from './SongApiContract';

	export default interface SongInListContract {

		order: number;

		notes: string;

		song: SongApiContract;

	}