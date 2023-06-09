import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';

/**
 * @deprecated The method should not be used
 */
export interface EntryWithTagUsagesContract {
	id: number;

	name: string;

	tags?: TagUsageForApiContract[];
}
