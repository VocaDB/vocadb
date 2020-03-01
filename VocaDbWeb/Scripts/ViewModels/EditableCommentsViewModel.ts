import CommentContract from '../DataContracts/CommentContract';
import CommentViewModel from './CommentViewModel';
import ICommentRepository from '../Repositories/ICommentRepository';
import ServerSidePagingViewModel from './ServerSidePagingViewModel';

//module vdb.viewModels {
	
	// Viewmodel for a list of comments where comments can be edited and new comments posted (with sufficient permissions).
	export default class EditableCommentsViewModel {

		constructor(
			private repo: ICommentRepository,			
			private entryId: number,
			private loggedUserId: number,
			private canDeleteAllComments: boolean,
			private canEditAllComments: boolean,
			private ascending: boolean,
			commentContracts?: CommentContract[],
			hasMoreComments: boolean = false) {
			
			this.comments = ko.observableArray<CommentViewModel>(null);
			this.commentsLoaded = commentContracts != null && !hasMoreComments;
			this.topComments = ko.computed(() => _.take(this.comments(), 3));
			this.pageOfComments = ko.computed(() => this.comments().slice(this.paging.firstItem(), this.paging.firstItem() + this.paging.pageSize()));

			if (commentContracts) {
				this.setComments(commentContracts);				
			}

		}
	
		public beginEditComment = (comment: CommentViewModel) => {

			comment.beginEdit();
			this.editCommentModel(comment);

		}

		public cancelEditComment = () => {
			this.editCommentModel(null);
		}

		private canDeleteComment = (comment: CommentContract) => {
			// If one can edit they can also delete
			return (this.canDeleteAllComments || this.canEditAllComments || (comment.author && comment.author.id === this.loggedUserId));
		}

		private canEditComment = (comment: CommentContract) => {
			return (this.canEditAllComments || (comment.author && comment.author.id === this.loggedUserId));
		}
			
		public comments: KnockoutObservableArray<CommentViewModel>;

		// Whether all comments have been loaded
		private commentsLoaded: boolean;

		public createComment = () => {

			var comment = this.newComment();

			if (!comment)
				return;

			this.newComment("");

			var commentContract: CommentContract = {
				author: { id: this.loggedUserId },
				message: comment
			}

			this.repo.createComment(this.entryId, commentContract, result => {

				var processed = this.processComment(result);
				this.paging.totalItems(this.paging.totalItems() + 1);

				if (this.ascending) {
					this.comments.push(processed);
					this.paging.goToLastPage();
				} else {
					this.comments.unshift(processed);
					this.paging.goToFirstPage();
				}

				if (this.onCommentCreated)
					this.onCommentCreated(_.clone(processed));

			});

		}

		public deleteComment = (comment: CommentViewModel) => {

			this.comments.remove(comment);

			this.repo.deleteComment(comment.id);
			this.paging.totalItems(this.paging.totalItems() - 1);

		}

		public editCommentModel = ko.observable<CommentViewModel>(null);

		public initComments = () => {

			if (this.commentsLoaded)
				return;

			this.repo.getComments(this.entryId, contracts => {
				this.setComments(contracts);				
			});

			this.commentsLoaded = true;

		}

		public newComment = ko.observable("");

		public onCommentCreated: (comment: CommentViewModel) => void;

		public paging: ServerSidePagingViewModel = new ServerSidePagingViewModel();

		public pageOfComments: KnockoutComputed<CommentViewModel[]>;

		private processComment = (contract: CommentContract) => {

			return new CommentViewModel(contract, this.canDeleteComment(contract), this.canEditComment(contract));

		}

		public saveEditedComment = () => {

			if (!this.editCommentModel())
				return;

			this.editCommentModel().saveChanges();
			var editedContract = this.editCommentModel().toContract();

			this.repo.updateComment(editedContract.id, editedContract);

			this.editCommentModel(null);

		}

		private setComments = (commentContracts: CommentContract[]) => {
			
			var commentViewModels = _.sortBy(_.map(commentContracts, comment => this.processComment(comment)), comment => comment.created);

			this.paging.totalItems(commentContracts.length);

			if (this.ascending)
				this.paging.goToLastPage();
			else
				commentViewModels = commentViewModels.reverse();

			this.comments(commentViewModels);

		}

		// Latest N comments
		public topComments: KnockoutComputed<CommentViewModel[]>;

	}

//} 