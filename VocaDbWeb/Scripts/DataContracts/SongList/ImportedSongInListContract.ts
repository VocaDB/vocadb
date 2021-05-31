import SongApiContract from '../Song/SongApiContract';

export default interface ImportedSongInListContract {
	matchedSong: SongApiContract;

	name: string;

	pvId: string;

	pvService: string;

	sortIndex: number;

	url: string;
}
