import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';

export default interface TagMappingContract {
	sourceTag: string;
	tag: TagBaseContract;
}
