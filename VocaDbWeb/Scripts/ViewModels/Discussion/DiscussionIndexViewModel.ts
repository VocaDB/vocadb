
module vdb.viewModels.discussions {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class DiscussionIndexViewModel {
		
		constructor(private repo: rep.DiscussionRepository) {
		
			repo.getFolders(folders => this.folders(folders));
				
			this.selectedFolder.subscribe(folder => {

				this.selectedTopic(null);

				if (folder === null) {
					return;
				}

				repo.getTopics(folder.id, topics => {
					this.topics(topics);

					_.each(topics, topic => {
						topic.canBeDeleted = false;
						_.each(topic.comments, comment => {
							comment.canBeDeleted = false;
						});
					});

				});

			});

		}

		public folders = ko.observableArray<dc.discussions.DiscussionFolderContract>([]);

		public selectedFolder = ko.observable<dc.discussions.DiscussionFolderContract>(null);

		public selectedTopic = ko.observable<dc.discussions.DiscussionTopicContract>(null);

		public topics = ko.observableArray<dc.discussions.DiscussionTopicContract>([]);

	}

} 