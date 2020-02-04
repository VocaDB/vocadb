
import ArtistContract from './Artist/ArtistContract';

//module vdb.dataContracts {

    export default interface ArtistForAlbumContract {

        artist: ArtistContract;

        id?: number;

		isCustomName?: boolean;

        isSupport?: boolean;

        name?: string;

        roles: string;

    }

//}
