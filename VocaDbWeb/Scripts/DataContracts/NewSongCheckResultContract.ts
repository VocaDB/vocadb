/// <reference path="DuplicateEntryResultContract.ts" />

import ArtistContract from './Artist/ArtistContract';
import DuplicateEntryResultContract from './DuplicateEntryResultContract';

//module vdb.dataContracts {

    export interface NewSongCheckResultContract {

		artists: ArtistContract[];

		matches: DuplicateEntryResultContract[];

		songType: string;

		title: string;

		titleLanguage: string; // TODO: content language selection

    }

//}