import ArtistContract from './Artist/ArtistContract';
import DuplicateEntryResultContract from './DuplicateEntryResultContract';

//module vdb.dataContracts {

    export default interface NewSongCheckResultContract {

		artists: ArtistContract[];

		matches: DuplicateEntryResultContract[];

		songType: string;

		title: string;

		titleLanguage: string; // TODO: content language selection

    }

//}