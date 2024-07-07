import { CommentContract } from '@/DataContracts/CommentContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { EntryType } from '@/Models/EntryType';
import { EventCategory } from '@/Models/Events/EventCategory';
import { LoginManager } from '@/Models/LoginManager';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { CommentRepository } from '@/Repositories/CommentRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import {
	UserOptionalField,
	UserRepository,
} from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { TagListStore } from '@/Stores/Tag/TagListStore';
import { TagsEditStore } from '@/Stores/Tag/TagsEditStore';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class ReleaseEventDetailsStore {
	readonly comments: EditableCommentsStore;
	@observable eventAssociationType?: UserEventRelationshipType;
	readonly reportStore: ReportEntryStore;
	readonly tagsEditStore: TagsEditStore;
	readonly tagUsages: TagListStore;
	@observable usersAttending: UserBaseContract[];

	constructor(
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
		eventCategory: EventCategory
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
			eventCategory
		);

		this.tagUsages = new TagListStore(tagUsages);
	}

	@computed get hasEvent(): boolean {
		return !!this.eventAssociationType;
	}

	@computed get isEventAttending(): boolean {
		return this.eventAssociationType === UserEventRelationshipType.Attending;
	}

	@computed get isEventInterested(): boolean {
		return this.eventAssociationType === UserEventRelationshipType.Interested;
	}

	@action removeEvent = (): void => {
		this.userRepo.deleteEventForUser({ eventId: this.eventId });
		this.eventAssociationType = undefined;
		const link = this.usersAttending.find(
			(u) => u.id === this.loginManager.loggedUserId,
		);
		pull(this.usersAttending, link);
	};

	@action setEventAttending = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Attending,
		});
		this.eventAssociationType = UserEventRelationshipType.Attending;
		this.userRepo
			.getOne({
				id: this.loginManager.loggedUserId,
				fields: [UserOptionalField.MainPicture],
			})
			.then((user) => {
				runInAction(() => {
					this.usersAttending.push(user);
				});
			});
	};

	@action setEventInterested = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Interested,
		});
		this.eventAssociationType = UserEventRelationshipType.Interested;
		const link = this.usersAttending.find(
			(u) => u.id === this.loginManager.loggedUserId,
		);
		pull(this.usersAttending, link);
	};
}
