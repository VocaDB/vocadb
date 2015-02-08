
module vdb.viewModels {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	// Viewmodel for a list of comments where comments can be edited and new comments posted (with sufficient permissions).
	export class EditableCommentsViewModel {

		constructor(
			private repo: rep.ICommentRepository,
			commentContracts: dc.CommentContract[],
			private entryId: number,
			private loggedUserId: number, private canDeleteAllComments: boolean) {
			
			var commentViewModels = _.sortBy(_.map(commentContracts, comment => this.processComment(comment)), comment => comment.created);

			this.comments = ko.observableArray<CommentViewModel>(commentViewModels);

		}
	
		public beginEditComment = (comment: CommentViewModel) => {

			comment.beginEdit();
			this.editCommentModel(comment);

		}

		public cancelEditComment = () => {
			this.editCommentModel(null);
		}

		private canEditOrDeleteComment = (comment: dc.CommentContract) => {
			return (this.canDeleteAllComments || (comment.author && comment.author.id === this.loggedUserId));
		}
			
		public comments: KnockoutObservableArray<CommentViewModel>;

		public createComment = () => {

			var comment = this.newComment();

			if (!comment)
				return;

			this.newComment("");

			var commentContract: dc.CommentContract = {
				author: { id: this.loggedUserId },
				message: comment
			}

			this.repo.createComment(this.entryId, commentContract, result => {
				this.comments.push(this.processComment(result));
			});

		}

		public deleteComment = (comment: CommentViewModel) => {

			this.comments.remove(comment);

			this.repo.deleteComment(comment.id);

		}

		public editCommentModel = ko.observable<CommentViewModel>(null);

		public newComment = ko.observable("");

		private processComment = (contract: dc.CommentContract) => {

			return new CommentViewModel(contract, this.canEditOrDeleteComment(contract), this.canEditOrDeleteComment(contract));

		}

		public saveEditedComment = () => {

			if (!this.editCommentModel())
				return;

			this.editCommentModel().saveChanges();
			var editedContract = this.editCommentModel().toContract();

			this.repo.updateComment(editedContract.id, editedContract);

			this.editCommentModel(null);

		}

	}

} 