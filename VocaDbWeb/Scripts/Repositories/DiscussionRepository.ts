
module vdb.repositories {
	
	import dc = vdb.dataContracts;

	export class DiscussionRepository {
		
		constructor(private urlMapper: vdb.UrlMapper) { }

		private mapUrl = (relative: string) => {
			return this.urlMapper.mapRelative(UrlMapper.mergeUrls("/api/discussions", relative));
		}

		public getFolders = (callback: (folders: dc.discussions.DiscussionFolderContract[]) => void) => {
			
			$.getJSON(this.mapUrl("folders"), callback);

		}

		public getTopics = (folderId: number, callback: (topics: dc.discussions.DiscussionTopicContract[]) => void) => {

			$.getJSON(this.mapUrl("folders/" + folderId + "/topics"), callback);

		}

	}

} 