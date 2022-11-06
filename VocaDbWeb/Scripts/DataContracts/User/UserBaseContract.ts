import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';

export interface UserBaseContract extends IEntryWithIdAndName {
	id: number;

	name?: string;
}
