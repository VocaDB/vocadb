import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';

export default interface TagUsageForApiContract {
	count: number;

	tag: TagBaseContract;
}
