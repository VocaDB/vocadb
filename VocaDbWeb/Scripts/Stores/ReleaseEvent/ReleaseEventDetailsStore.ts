import CommentContract from '@DataContracts/CommentContract';
import TagSelectionContract from '@DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import UserBaseContract from '@DataContracts/User/UserBaseContract';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import UserEventRelationshipType from '@Models/Users/UserEventRelationshipType';
import CommentRepository from '@Repositories/CommentRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import EditableCommentsStore from '@Stores/EditableCommentsStore';
import ReportEntryStore from '@Stores/ReportEntryStore';
import TagListStore from '@Stores/Tag/TagListStore';
import TagsEditStore from '@Stores/Tag/TagsEditStore';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export default class ReleaseEventDetailsStore {
	public readonly comments: EditableCommentsStore;
	@observable public eventAssociationType?: UserEventRelationshipType;
	public readonly reportStore: ReportEntryStore;
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagUsages: TagListStore;
	@observable public usersAttending: UserBaseContract[];

	public constructor(
		private readonly loginManager: LoginManager,
		httpClient: HttpClient,
		urlMapper: UrlMapper,
		eventRepo: ReleaseEventRepository,
		private readonly userRepo: UserRepository,
		latestComments: CommentContract[],
		private readonly eventId: number,
		eventAssociationType: UserEventRelationshipType,
		usersAttending: UserBaseContract[],
		tagUsages: TagUsageForApiContract[],
		canDeleteAllComments: boolean,
	) {
		makeObservable(this);

		const commentRepo = new CommentRepository(
			httpClient,
			urlMapper,
			EntryType.ReleaseEvent,
		);
		this.comments = new EditableCommentsStore(
			loginManager,
			commentRepo,
			eventId,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			latestComments,
			true,
		);
		this.eventAssociationType = eventAssociationType;
		this.usersAttending = usersAttending;

		this.reportStore = new ReportEntryStore(
			(reportType, notes): Promise<void> => {
				return eventRepo.createReport({
					entryId: eventId,
					reportType: reportType,
					notes: notes,
					versionNumber: undefined,
				});
			},
		);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getEventTagSelections({ eventId: this.eventId }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateEventTags({ eventId: this.eventId, tags: tags })
						.then(this.tagUsages.updateTagUsages),
			},
			EntryType.ReleaseEvent,
		);

		this.tagUsages = new TagListStore(tagUsages);
	}

	@computed public get hasEvent(): boolean {
		return !!this.eventAssociationType;
	}

	@computed public get isEventAttending(): boolean {
		return this.eventAssociationType === UserEventRelationshipType.Attending;
	}

	@computed public get isEventInterested(): boolean {
		return this.eventAssociationType === UserEventRelationshipType.Interested;
	}

	@action public removeEvent = (): void => {
		this.userRepo.deleteEventForUser({ eventId: this.eventId });
		this.eventAssociationType = undefined;
		const link = _.find(
			this.usersAttending,
			(u) => u.id === this.loginManager.loggedUserId,
		);
		_.pull(this.usersAttending, link);
	};

	@action public setEventAttending = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Attending,
		});
		this.eventAssociationType = UserEventRelationshipType.Attending;
		this.userRepo
			.getOne({
				id: this.loginManager.loggedUserId,
				fields: 'MainPicture',
			})
			.then((user) => {
				runInAction(() => {
					this.usersAttending.push(user);
				});
			});
	};

	@action public setEventInterested = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Interested,
		});
		this.eventAssociationType = UserEventRelationshipType.Interested;
		const link = _.find(
			this.usersAttending,
			(u) => u.id === this.loginManager.loggedUserId,
		);
		_.pull(this.usersAttending, link);
	};
}
