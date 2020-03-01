import EntryRefContract from './EntryRefContract';

//module vdb.dataContracts {

    export default interface DuplicateEntryResultContract {

        entry: EntryRefWithNameContract;

        matchProperty: string;

    }

    export interface EntryNameContract {

        additionalNames: string;

        displayName: string;

    }

    export interface EntryRefWithNameContract extends EntryRefContract {

        name: EntryNameContract;

    }


    export interface EntryRefWithCommonPropertiesContract extends EntryRefWithNameContract {

        artistString: string;

        entryTypeName: string;

    }

//}
