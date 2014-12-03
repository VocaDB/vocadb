/// <reference path="ArtistContract.ts" />

module vdb.dataContracts {

    export interface SongContract {

        additionalNames: string;

        artistString: string;

        id: number;

		lengthSeconds: number;

		name: string;

		songType: string;

		thumbUrl?: string;

    }

}

