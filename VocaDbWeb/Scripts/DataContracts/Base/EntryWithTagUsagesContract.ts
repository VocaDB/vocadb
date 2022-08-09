import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';

export interface EntryWithTagUsagesContract {
	id: number;

	name: string;

	tags?: TagUsageForApiContract[];
}
