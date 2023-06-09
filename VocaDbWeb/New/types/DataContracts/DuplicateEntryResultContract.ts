import { EntryRefContract } from '@/types/DataContracts/EntryRefContract';

export interface DuplicateEntryResultContract {
	entry: EntryRefWithNameContract & {
		artistString?: string;

		entryTypeName: string;
	};

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
