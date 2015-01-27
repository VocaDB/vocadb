
module vdb.dataContracts.discussions {
	
	export interface DiscussionFolderContract {
		
		description: string;

		id: number;

		lastTopicDate: Date;

		name: string;

		topicCount: number;

	}

} 