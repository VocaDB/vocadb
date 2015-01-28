
module vdb.viewModels.discussions {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class DiscussionIndexViewModel {
		
		constructor(private repo: rep.DiscussionRepository, private loggedUserId: number) {
		
			this.newTopic = ko.observable(new DiscussionTopicEditViewModel(loggedUserId));


			repo.getFolders(folders => this.folders(folders));
				
			this.selectedFolder.subscribe(folder => {

				this.showCreateNewTopic(false);
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

		public createNewTopic = () => {
			
			var folder = this.selectedFolder();
			this.repo.createTopic(folder.id, this.newTopic().toContract(), topic => {

				topic.canBeDeleted = false;
				this.newTopic(new DiscussionTopicEditViewModel(this.loggedUserId));
				this.showCreateNewTopic(false);
				this.topics.unshift(topic);
				this.selectedTopic(topic);

			});

		}

		public folders = ko.observableArray<dc.discussions.DiscussionFolderContract>([]);

		public newTopic: KnockoutObservable<DiscussionTopicEditViewModel>;

		public selectedFolder = ko.observable<dc.discussions.DiscussionFolderContract>(null);

		public selectedTopic = ko.observable<dc.discussions.DiscussionTopicContract>(null);

		public showCreateNewTopic = ko.observable(false);

		public topics = ko.observableArray<dc.discussions.DiscussionTopicContract>([]);

	}

	export class DiscussionTopicEditViewModel {
		
		constructor(userId: number) {
			this.author = { id: userId, name: '' };
		}

		public author: dc.UserWithIconContract;

		public content = ko.observable("");

		public name = ko.observable("");

		public toContract = (): dc.discussions.DiscussionTopicContract => {
			return ko.toJS(this);
		}

	}

} 