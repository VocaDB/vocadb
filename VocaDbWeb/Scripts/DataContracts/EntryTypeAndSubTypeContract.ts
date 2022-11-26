import { EntryType } from '@/Models/EntryType';

export interface EntryTypeAndSubTypeContract {
	entryType: EntryType;
	hasSubType?: boolean;
	hasValue?: boolean;
	subType: string;
}
