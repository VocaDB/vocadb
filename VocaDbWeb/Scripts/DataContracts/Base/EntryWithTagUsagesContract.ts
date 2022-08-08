import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';

export default interface EntryWithTagUsagesContract {
	id: number;

	name: string;

	tags?: TagUsageForApiContract[];
}
