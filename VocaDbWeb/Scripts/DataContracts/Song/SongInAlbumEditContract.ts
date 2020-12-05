import ArtistContract from '../Artist/ArtistContract';

	export default interface SongInAlbumEditContract {

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