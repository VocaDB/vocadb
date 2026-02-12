import { CommentContract } from '@/DataContracts/CommentContract';
import { LoginManager } from '@/Models/LoginManager';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { EnglishTranslatedStringStore } from '@/Stores/Globalization/EnglishTranslatedStringStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { action, makeObservable, observable } from 'mobx';

export class TagDetailsStore {
	readonly comments: EditableCommentsStore;
	@observable isFollowed: boolean;
	readonly reportStore: ReportEntryStore;
	readonly description: EnglishTranslatedStringStore;

	constructor(
		private readonly loginManager: LoginManager,
		tagRepo: TagRepository,
		private readonly userRepo: UserRepository,
		latestComments: CommentContract[],
		private readonly tagId: number,
		commentsLocked: boolean,
		canDeleteAllComments: boolean,
		showTranslatedDescription: boolean,
		isFollowed: boolean,
	) {
		makeObservable(this);

		this.comments = new EditableCommentsStore(
			loginManager,
			tagRepo.getComments({}),
			tagId,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			latestComments,
			true,
			commentsLocked,
		);

		this.reportStore = new ReportEntryStore((reportType, notes) => {
			return tagRepo.createReport({
				entryId: tagId,
				reportType: reportType,
				notes: notes,
				versionNumber: undefined,
			});
		});

		this.description = new EnglishTranslatedStringStore(
			showTranslatedDescription,
		);
		this.isFollowed = isFollowed;
	}

	@action followTag = (): void => {
		if (!this.loginManager.isLoggedIn) return;
		this.userRepo.addFollowedTag({ tagId: this.tagId });
		this.isFollowed = true;
	};

	@action unfollowTag = (): void => {
		this.userRepo.deleteFollowedTag({ tagId: this.tagId });
		this.isFollowed = false;
	};
}
