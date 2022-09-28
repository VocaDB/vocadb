import { EntryStatus } from '@/Models/EntryStatus';

export interface CommonEntryContract {
	createDate?: string;
	id: number;
	name: string;
	status: EntryStatus;
	version?: number;
}
