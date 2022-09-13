import { CommentContract } from '@/DataContracts/CommentContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { EntryType } from '@/Models/EntryType';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { CommentRepository } from '@/Repositories/CommentRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import {
	UserOptionalField,
	UserRepository,
} from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { HttpClient } from '@/Shared/HttpClient';
import { ui } from '@/Shared/MessagesTyped';
import { UrlMapper } from '@/Shared/UrlMapper';
import { EditableCommentsViewModel } from '@/ViewModels/EditableCommentsViewModel';
import {
	IEntryReportType,
	ReportEntryViewModel,
} from '@/ViewModels/ReportEntryViewModel';
import { TagListViewModel } from '@/ViewModels/Tag/TagListViewModel';
import { TagsEditViewModel } from '@/ViewModels/Tag/TagsEditViewModel';
import ko, { ObservableArray } from 'knockout';

export class ReleaseEventDetailsViewModel {
	public constructor(
		private readonly values: GlobalValues,
		httpClient: HttpClient,
		urlMapper: UrlMapper,
		private readonly repo: ReleaseEventRepository,
		private readonly userRepo: UserRepository,
		latestComments: CommentContract[],
		reportTypes: IEntryReportType[],
		/* TODO: remove */ public loggedUserId: number,
		private readonly eventId: number,
		eventAssociationType: UserEventRelationshipType,
		usersAttending: UserBaseContract[],
		tagUsages: TagUsageForApiContract[],
		canDeleteAllComments: boolean,
	) {
		const commentRepo = new CommentRepository(
			httpClient,
			urlMapper,
			EntryType.ReleaseEvent,
		);
		this.comments = new EditableCommentsViewModel(
			values,
			commentRepo,
			eventId,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			latestComments,
			true,
		);
		this.eventAssociationType(eventAssociationType);
		this.usersAttending = ko.observableArray(usersAttending);

		this.reportViewModel = new ReportEntryViewModel(
			reportTypes,
			(reportType, notes) => {
				repo.createReport({
					entryId: eventId,
					reportType: reportType,
					notes: notes,
					versionNumber: undefined,
				});
				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			},
		);

		this.tagsEditViewModel = new TagsEditViewModel(
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

		this.tagUsages = new TagListViewModel(tagUsages);
	}

	public comments: EditableCommentsViewModel;

	private eventAssociationType = ko.observable<UserEventRelationshipType>(
		null!,
	);

	public hasEvent = ko.computed(() => {
		return !!this.eventAssociationType();
	});

	public isEventAttending = ko.computed(
		() => this.eventAssociationType() === UserEventRelationshipType.Attending,
	);

	public isEventInterested = ko.computed(
		() => this.eventAssociationType() === UserEventRelationshipType.Interested,
	);

	public removeEvent = (): void => {
		this.userRepo.deleteEventForUser({ eventId: this.eventId });
		this.eventAssociationType(null!);
		var link = this.usersAttending().find(
			(u) => u.id === this.values.loggedUserId,
		)!;
		this.usersAttending.remove(link);
	};

	public reportViewModel: ReportEntryViewModel;

	public setEventAttending = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Attending,
		});
		this.eventAssociationType(UserEventRelationshipType.Attending);
		this.userRepo
			.getOne({
				id: this.values.loggedUserId,
				fields: [UserOptionalField.MainPicture],
			})
			.then((user) => {
				this.usersAttending.push(user);
			});
	};

	public setEventInterested = (): void => {
		this.userRepo.updateEventForUser({
			eventId: this.eventId,
			associationType: UserEventRelationshipType.Interested,
		});
		this.eventAssociationType(UserEventRelationshipType.Interested);
		var link = this.usersAttending().find(
			(u) => u.id === this.values.loggedUserId,
		)!;
		this.usersAttending.remove(link);
	};

	public tagsEditViewModel: TagsEditViewModel;

	public tagUsages: TagListViewModel;

	public usersAttending: ObservableArray<UserBaseContract>;
}
