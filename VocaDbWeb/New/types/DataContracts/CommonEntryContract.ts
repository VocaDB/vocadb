import { EntryStatus } from '@/types/Models/EntryStatus';

export interface CommonEntryContract {
	createDate?: string;
	id: number;
	name: string;
	status: EntryStatus;
	version?: number;
}
