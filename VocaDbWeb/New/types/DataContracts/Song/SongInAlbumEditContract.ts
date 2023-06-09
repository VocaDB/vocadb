import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';

export interface SongInAlbumEditContract {
	artists: ArtistContract[];

	artistString: string;

	discNumber: number;

	isCustomTrack?: boolean;

	songAdditionalNames: string;

	songId: number;

	songInAlbumId: number;

	songName: string;

	trackNumber: number;
}
