import { CommentContract } from '@/DataContracts/CommentContract';
import { LoginManager } from '@/Models/LoginManager';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { CommentStore } from '@/Stores/CommentStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { clone, pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

// Store for a list of comments where comments can be edited and new comments posted (with sufficient permissions).
export class EditableCommentsStore {
	@observable comments: CommentStore[] = [];
	// Whether all comments have been loaded
	@observable commentsLoaded = false;
	@observable commentsLocked = false;
	@observable editCommentStore?: CommentStore = undefined;
	@observable newComment = '';
	readonly paging = new ServerSidePagingStore();

	constructor(
		private readonly loginManager: LoginManager,
		private readonly commentRepo: ICommentRepository,
		private readonly entryId: number,
		private readonly canDeleteAllComments: boolean,
		private readonly canEditAllComments: boolean,
		private readonly ascending: boolean,
		commentContracts?: CommentContract[],
		hasMoreComments: boolean = false,
		commentsLocked: boolean = false,
	) {
		makeObservable(this);

		this.commentsLocked = commentsLocked;
		this.commentsLoaded = !!commentContracts && !hasMoreComments;

		if (commentContracts) {
			this.setComments(commentContracts);
		}
	}

	@computed get pageOfComments(): CommentStore[] {
		return this.comments.slice(
			this.paging.firstItem,
			this.paging.firstItem + this.paging.pageSize,
		);
	}

	// Latest N comments
	@computed get topComments(): CommentStore[] {
		return this.comments.take(3);
	}

	@action beginEditComment = (comment: CommentStore): void => {
		comment.beginEdit();
		this.editCommentStore = comment;
	};

	@action cancelEditComment = (): void => {
		this.editCommentStore = undefined;
	};

	private canDeleteComment = (comment: CommentContract): boolean => {
		// If one can edit they can also delete
		return (
			this.canDeleteAllComments ||
			this.canEditAllComments ||
			(comment.author && comment.author.id === this.loginManager.loggedUserId)
		);
	};

	private canEditComment = (comment: CommentContract): boolean => {
		return (
			this.canEditAllComments ||
			(comment.author && comment.author.id === this.loginManager.loggedUserId)
		);
	};

	private processComment = (contract: CommentContract): CommentStore => {
		return new CommentStore(
			contract,
			this.canDeleteComment(contract),
			this.canEditComment(contract),
		);
	};

	@action createComment = (): Promise<CommentStore> => {
		const comment = this.newComment;

		if (!comment) return Promise.reject();

		this.newComment = '';

		const commentContract: CommentContract = {
			author: { id: this.loginManager.loggedUserId },
			created: undefined!,
			message: comment,
		};

		return this.commentRepo
			.createComment({
				entryId: this.entryId,
				contract: commentContract,
			})
			.then((result) => {
				const processed = this.processComment(result);
				runInAction(() => {
					this.paging.totalItems = this.paging.totalItems + 1;

					if (this.ascending) {
						this.comments.push(processed);
						this.paging.goToLastPage();
					} else {
						this.comments.unshift(processed);
						this.paging.goToFirstPage();
					}
				});

				return clone(processed);
			});
	};

	@action deleteComment = (comment: CommentStore): void => {
		pull(this.comments, comment);

		this.commentRepo.deleteComment({ commentId: comment.id! });
		this.paging.totalItems = this.paging.totalItems - 1;
	};

	@action private setComments = (commentContracts: CommentContract[]): void => {
		var commentStores = commentContracts
			.map((comment) => this.processComment(comment))
			.sortBy((comment) => comment.created);

		this.paging.totalItems = commentContracts.length;

		if (this.ascending) {
			//this.paging.goToLastPage();
		} else {
			commentStores = commentStores.reverse();
		}

		this.comments = commentStores;
	};

	@action initComments = (): void => {
		if (this.commentsLoaded) return;

		this.commentRepo
			.getComments({ entryId: this.entryId })
			.then((contracts) => {
				this.setComments(contracts);
			});

		this.commentsLoaded = true;
	};

	@action toggleCommentsLocked = async (): Promise<void> => {
		const newLocked = !this.commentsLocked;
		await this.commentRepo.setCommentsLocked({
			entryId: this.entryId,
			locked: newLocked,
		});
		runInAction(() => {
			this.commentsLocked = newLocked;
		});
	};

	@action saveEditedComment = (): void => {
		if (!this.editCommentStore) return;

		this.editCommentStore.saveChanges();
		const editedContract = this.editCommentStore.toContract();

		this.commentRepo.updateComment({
			commentId: editedContract.id!,
			contract: editedContract,
		});

		this.editCommentStore = undefined;
	};
}
