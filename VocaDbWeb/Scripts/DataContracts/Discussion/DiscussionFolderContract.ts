import { UserApiContract } from '@/DataContracts/User/UserApiContract';

export interface DiscussionFolderContract {
	description: string;

	id: number;

	lastTopicAuthor?: UserApiContract;

	lastTopicDate: Date;

	name: string;

	topicCount: number;
}
