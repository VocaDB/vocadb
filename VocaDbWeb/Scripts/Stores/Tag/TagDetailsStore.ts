import CommentContract from '@DataContracts/CommentContract';
import LoginManager from '@Models/LoginManager';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import EditableCommentsStore from '@Stores/EditableCommentsStore';
import EnglishTranslatedStringStore from '@Stores/Globalization/EnglishTranslatedStringStore';
import ReportEntryStore from '@Stores/ReportEntryStore';
import { action, makeObservable, observable } from 'mobx';

export default class TagDetailsStore {
	public readonly comments: EditableCommentsStore;
	@observable public isFollowed: boolean;
	public readonly reportStore: ReportEntryStore;
	public readonly description: EnglishTranslatedStringStore;

	public constructor(
		private readonly loginManager: LoginManager,
		tagRepo: TagRepository,
		private readonly userRepo: UserRepository,
		latestComments: CommentContract[],
		private readonly tagId: number,
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

	@action public followTag = (): void => {
		if (!this.loginManager.isLoggedIn) return;
		this.userRepo.addFollowedTag({ tagId: this.tagId });
		this.isFollowed = true;
	};

	@action public unfollowTag = (): void => {
		this.userRepo.deleteFollowedTag({ tagId: this.tagId });
		this.isFollowed = false;
	};
}
