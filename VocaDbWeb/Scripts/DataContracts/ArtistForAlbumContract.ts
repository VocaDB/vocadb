
module vdb.dataContracts {

    export interface ArtistForAlbumContract {

        artist: ArtistContract;

        id?: number;

        isSupport?: boolean;

        name?: string;

        roles: string;

    }

}
