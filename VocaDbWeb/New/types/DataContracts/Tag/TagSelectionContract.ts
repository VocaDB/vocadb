import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';

export interface TagSelectionContract {
	selected?: boolean;

	tag: TagBaseContract;
}
