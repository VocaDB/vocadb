import { TagUsageWithVotesForApiContract } from '../Tag/TagUsageWithVotesForApiContract';

export interface EntryWithTagUsagesForApiContract {
	canRemoveTagUsages: boolean;

	id: number;

	defaultName: string;

	tagUsages: TagUsageWithVotesForApiContract[];
}
