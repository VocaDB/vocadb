import { EntryTypeAndSubTypeContract } from '@/types/DataContracts/EntryTypeAndSubTypeContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';

export interface EntryTagMappingContract {
	entryType: EntryTypeAndSubTypeContract;
	tag: TagBaseContract;
}
