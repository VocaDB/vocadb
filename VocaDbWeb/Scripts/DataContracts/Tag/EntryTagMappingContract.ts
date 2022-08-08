import EntryTypeAndSubTypeContract from '@/DataContracts/EntryTypeAndSubTypeContract';
import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';

export default interface EntryTagMappingContract {
	entryType: EntryTypeAndSubTypeContract;
	tag: TagBaseContract;
}
