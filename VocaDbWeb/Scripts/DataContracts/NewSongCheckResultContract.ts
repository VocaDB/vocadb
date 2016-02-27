/// <reference path="DuplicateEntryResultContract.ts" />

module vdb.dataContracts {

    export interface NewSongCheckResultContract {

		artists: ArtistContract[];

		matches: DuplicateEntryResultContract[];

		songType: string;

		title: string;

		titleLanguage: string; // TODO: content language selection

    }

}