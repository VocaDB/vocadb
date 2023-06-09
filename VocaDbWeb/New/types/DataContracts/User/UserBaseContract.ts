import { IEntryWithIdAndName } from '@/types/Models/IEntryWithIdAndName';

export interface UserBaseContract extends IEntryWithIdAndName {
	id: number;

	name?: string;
}
