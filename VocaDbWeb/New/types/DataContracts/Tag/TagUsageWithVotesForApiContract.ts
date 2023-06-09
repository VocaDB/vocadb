import { UserApiContract } from '../User/UserApiContract';
import { TagBaseContract } from './TagBaseContract';

export interface TagUsageWithVotesForApiContract {
	date: string;
	count: number;
	id: number;
	tag: TagBaseContract;
	votes: UserApiContract[];
}
