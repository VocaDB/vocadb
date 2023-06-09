import { EntryStatus } from '@/types/Models/EntryStatus';

export interface TagBaseContract {
	// Additional names list - optional field
	additionalNames?: string;
	categoryName?: string;
	id: number;
	name: string;
	status: EntryStatus;
	urlSlug?: string;
}
