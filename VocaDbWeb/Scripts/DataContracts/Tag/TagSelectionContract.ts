import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';

export default interface TagSelectionContract {
	selected?: boolean;

	tag: TagBaseContract;
}
