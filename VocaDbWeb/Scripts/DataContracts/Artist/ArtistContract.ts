import CommonEntryContract from '../CommonEntryContract';

//module vdb.dataContracts {

    export default interface ArtistContract extends CommonEntryContract {

        additionalNames?: string;

        artistType?: string;

    }

//}