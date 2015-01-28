
module vdb.repositories {
	
	import dc = vdb.dataContracts;

	export class DiscussionRepository {
		
		constructor(private urlMapper: vdb.UrlMapper) { }

		private mapUrl = (relative: string) => {
			return this.urlMapper.mapRelative(UrlMapper.mergeUrls("/api/discussions", relative));
		}

		public createComment = (folderId: number, topicId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			$.post(this.mapUrl("topics/" + topicId + "/comments"), contract, callback, 'json');

		}

		public createTopic = (folderId: number, contract: dc.discussions.DiscussionTopicContract, callback: (contract: dc.discussions.DiscussionTopicContract) => void) => {

			$.post(this.mapUrl("folders/" + folderId + "/topics"), contract, callback, 'json');

		}

		public getFolders = (callback: (folders: dc.discussions.DiscussionFolderContract[]) => void) => {
			
			$.getJSON(this.mapUrl("folders"), callback);

		}

		public getTopics = (folderId: number, callback: (topics: dc.discussions.DiscussionTopicContract[]) => void) => {

			$.getJSON(this.mapUrl("folders/" + folderId + "/topics"), callback);

		}

		public updateTopic = (topicId: number, contract: dc.discussions.DiscussionTopicContract, callback: () => void) => {

			$.post(this.mapUrl("topics/" + topicId), contract, callback, 'json');

		}

	}

} 