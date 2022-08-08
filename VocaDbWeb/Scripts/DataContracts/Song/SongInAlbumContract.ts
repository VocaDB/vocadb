import SongApiContract from '@/DataContracts/Song/SongApiContract';

export default interface SongInAlbumContract {
	discNumber: number;
	id: number;
	name: string;
	rating?: string;
	song: SongApiContract;
	trackNumber: number;
}
