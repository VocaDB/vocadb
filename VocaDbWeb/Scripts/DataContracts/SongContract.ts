/// <reference path="ArtistContract.ts" />

module vdb.dataContracts {

    export interface SongContract {

        additionalNames: string;

        artistString: string;

        id: number;

		lengthSeconds: number;

        name: string;

		thumbUrl?: string;

    }

	// TODO: migrate to SongForApi, extract SongWithPVAndVoteContract
    export interface SongWithComponentsContract extends SongContract {
        
        artists?: ArtistContract[];

        vote: string;

    }

}

