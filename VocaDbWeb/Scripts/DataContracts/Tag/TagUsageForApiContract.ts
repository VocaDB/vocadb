import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';

export interface TagUsageForApiContract {
	count: number;

	tag: TagBaseContract;
}
