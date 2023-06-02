import { SongApiContract } from '@/DataContracts/Song/SongApiContract';

export interface SongInAlbumContract {
	discNumber: number;
	id: number;
	name: string;
	rating?: string;
	song?: SongApiContract;
	trackNumber: number;
	computedCultureCodes?: string[];
}
