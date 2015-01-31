
module vdb.viewModels.discussions {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class DiscussionIndexViewModel {
		
		constructor(private repo: rep.DiscussionRepository,
			private urlMapper: vdb.UrlMapper,
			private canDeleteAllComments: boolean,
			private loggedUserId: number) {
		
			this.newTopic = ko.observable(new DiscussionTopicEditViewModel(loggedUserId));


			repo.getFolders(folders => this.folders(folders));
			repo.getTopics(result => this.recentTopics(result.items));
				
			this.selectedFolder.subscribe(folder => {

				this.showCreateNewTopic(false);
				this.selectedTopic(null);

				this.loadTopics(folder);

			});

		}

		private canDeleteTopic = (topic: dc.discussions.DiscussionTopicContract) => {
			return (this.canDeleteAllComments || (topic.author && topic.author.id === this.loggedUserId));
		}

		private canEditTopic = (topic: dc.discussions.DiscussionTopicContract) => {
			return (this.canDeleteAllComments || (topic.author && topic.author.id === this.loggedUserId));
		}

		public createNewTopic = () => {
			
			var folder = this.selectedFolder();
			this.repo.createTopic(folder.id, this.newTopic().toContract(), topic => {

				topic.canBeDeleted = false;
				this.newTopic(new DiscussionTopicEditViewModel(this.loggedUserId));
				this.showCreateNewTopic(false);
				this.topics.unshift(topic);
				this.selectTopic(topic);

			});

		}

		public deleteTopic = (topic: dc.discussions.DiscussionTopicContract) => {
			
			this.repo.deleteTopic(topic.id, () => {
				this.selectTopic(null);				
			});

		}

		public folders = ko.observableArray<dc.discussions.DiscussionFolderContract>([]);

		private getFolder = (folderId: number) => {
			return _.find(this.folders(), f => f.id === folderId);
		}

		private loadTopics = (folder: dc.discussions.DiscussionFolderContract, callback?: () => void) => {
		
			if (!folder) {

				this.topics([]);

				if (callback)
					callback();

				return;

			}

			this.repo.getTopicsForFolder(folder.id, topics => {

				this.topics(topics);

				if (callback)
					callback();

			});

		}

		public newTopic: KnockoutObservable<DiscussionTopicEditViewModel>;

		public recentTopics = ko.observableArray<dc.discussions.DiscussionTopicContract>([]);

		public selectTopic = (topic: dc.discussions.DiscussionTopicContract) => {
			
			if (!topic) {
				this.loadTopics(this.selectedFolder(), () => this.selectedTopic(null));
				return;
			}

			this.repo.getTopic(topic.id, contract => {

				contract.canBeDeleted = this.canDeleteTopic(contract);
				contract.canBeEdited = this.canEditTopic(contract);

				this.selectedFolder(this.getFolder(contract.folderId));
				this.selectedTopic(new DiscussionTopicViewModel(this.repo, this.loggedUserId, this.canDeleteAllComments, contract));

			});			

		}

		public selectedFolder = ko.observable<dc.discussions.DiscussionFolderContract>(null);

		public selectedTopic = ko.observable<DiscussionTopicViewModel>(null);

		public showCreateNewTopic = ko.observable(false);

		public topics = ko.observableArray<dc.discussions.DiscussionTopicContract>([]);

	}

	export class DiscussionTopicViewModel {
		
		constructor(private repo: rep.DiscussionRepository, private loggedUserId: number,
			private canDeleteAllComments: boolean,
			contract: dc.discussions.DiscussionTopicContract) {

			this.contract = ko.observable(contract);

			_.each(contract.comments, comment => {
				comment.canBeDeleted = this.canDeleteComment(comment);
			});
			
			this.comments = ko.observableArray<dc.CommentContract>(contract.comments || []);

		}

		public beginEditTopic = () => {
			this.editModel(new DiscussionTopicEditViewModel(this.loggedUserId, this.contract()));
		}

		public cancelEdit = () => {
			this.editModel(null);
		}

		private canDeleteComment = (comment: dc.CommentContract) => {
			return (this.canDeleteAllComments || (comment.author && comment.author.id === this.loggedUserId));
		}

		public comments: KnockoutObservableArray<dc.CommentContract>;

		public contract: KnockoutObservable<dc.discussions.DiscussionTopicContract>;

		public createComment = () => {

			var comment = this.newComment();

			if (!comment)
				return;

			this.newComment("");

			var commentContract: dc.CommentContract = {
				author: { id: this.loggedUserId },
				message: comment
			}

			this.repo.createComment(this.contract().id, commentContract, result => {
				this.processComment(result);
				this.comments.unshift(result);
			});


		}

		public deleteComment = (comment: dc.CommentContract) => {
			
			this.comments.remove(comment);

			this.repo.deleteComment(comment.id);

		}

		public editModel = ko.observable<DiscussionTopicEditViewModel>(null);

		public isBeingEdited = ko.computed(() => this.editModel() !== null);

		public newComment = ko.observable("");

		private processComment = (comment: dc.CommentContract) => {

			comment.canBeDeleted = this.canDeleteComment(comment);

		}

		public saveEditedTopic = () => {
			
			if (!this.isBeingEdited())
				return;

			var editedContract = this.editModel().toContract();

			this.repo.updateTopic(this.contract().id, editedContract, () => {

				editedContract.id = this.contract().id;
				editedContract.created = this.contract().created;
				editedContract.canBeDeleted = this.contract().canBeDeleted;
				editedContract.canBeEdited = this.contract().canBeEdited;

				this.contract(editedContract);
				this.editModel(null);

			});

		}

	}

	export class DiscussionTopicEditViewModel {
		
		constructor(userId: number, contract?: dc.discussions.DiscussionTopicContract) {

			this.author = { id: userId, name: '' };

			if (contract) {
				this.author = contract.author;
				this.content(contract.content);
				this.name(contract.name);
			}

		}

		public author: dc.UserWithIconContract;

		public content = ko.observable("");

		public name = ko.observable("");

		public toContract = (): dc.discussions.DiscussionTopicContract => {
			return ko.toJS(this);
		}

	}

} 