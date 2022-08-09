import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';

export interface TagSelectionContract {
	selected?: boolean;

	tag: TagBaseContract;
}
