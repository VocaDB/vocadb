import { EntryType } from '@/types/Models/EntryType';

export interface EntryTypeAndSubTypeContract {
	entryType: EntryType;
	hasSubType?: boolean;
	hasValue?: boolean;
	subType: string;
}
