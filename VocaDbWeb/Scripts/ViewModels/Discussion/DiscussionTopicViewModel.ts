
module vdb.viewModels.discussions {
	
	import dc = vdb.dataContracts;

	export class DiscussionTopicViewModel {

		constructor(private repo: rep.DiscussionRepository, private loggedUserId: number,
			private canDeleteAllComments: boolean,
			contract: dc.discussions.DiscussionTopicContract) {

			this.contract = ko.observable(contract);

			_.each(contract.comments, comment => {
				this.processComment(comment);
			});

			this.comments = ko.observableArray<dc.CommentContract>(contract.comments || []);

		}

		public beginEditTopic = () => {
			this.editModel(new DiscussionTopicEditViewModel(this.loggedUserId, this.contract()));
		}

		public cancelEdit = () => {
			this.editModel(null);
		}

		private canEditOrDeleteComment = (comment: dc.CommentContract) => {
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

			comment.canBeDeleted = this.canEditOrDeleteComment(comment);

		}

		public saveEditedTopic = () => {

			if (!this.isBeingEdited())
				return;

			var editedContract = this.editModel().toContract();

			this.repo.updateTopic(this.contract().id, editedContract,() => {

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