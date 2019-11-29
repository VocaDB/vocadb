
import CommentContract from '../../DataContracts/CommentContract';
import CommentRepository from '../../Repositories/CommentRepository';
import EditableCommentsViewModel from '../EditableCommentsViewModel';
import EntryType from '../../Models/EntryType';
import { IEntryReportType } from '../ReportEntryViewModel';
import ReleaseEventRepository from '../../Repositories/ReleaseEventRepository';
import ReportEntryViewModel from '../ReportEntryViewModel';
import TagListViewModel from '../Tag/TagListViewModel';
import TagsEditViewModel from '../Tag/TagsEditViewModel';
import TagUsageForApiContract from '../../DataContracts/Tag/TagUsageForApiContract';
import ui from '../../Shared/MessagesTyped';
import UrlMapper from '../../Shared/UrlMapper';
import UserBaseContract from '../../DataContracts/User/UserBaseContract';
import UserEventRelationshipType from '../../Models/Users/UserEventRelationshipType';
import UserRepository from '../../Repositories/UserRepository';

//namespace vdb.viewModels.releaseEvents {

	export class ReleaseEventDetailsViewModel {

		constructor(
			urlMapper: UrlMapper,
			private readonly repo: ReleaseEventRepository,
			private readonly userRepo: UserRepository,
			latestComments: CommentContract[],
			reportTypes: IEntryReportType[],
			public loggedUserId: number,
			private readonly eventId: number,
			eventAssociationType: UserEventRelationshipType,
			usersAttending: UserBaseContract[],
			tagUsages: TagUsageForApiContract[],
			canDeleteAllComments: boolean) {

			const commentRepo = new CommentRepository(urlMapper, EntryType.ReleaseEvent);
			this.comments = new EditableCommentsViewModel(commentRepo, eventId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);
			this.eventAssociationType(eventAssociationType);
			this.usersAttending = ko.observableArray(usersAttending);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {
				repo.createReport(eventId, reportType, notes, null);
				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			});

			this.tagsEditViewModel = new TagsEditViewModel({
				getTagSelections: callback => userRepo.getEventTagSelections(this.eventId, callback),
				saveTagSelections: tags => userRepo.updateEventTags(this.eventId, tags, this.tagUsages.updateTagUsages)
			}, EntryType.ReleaseEvent);

			this.tagUsages = new TagListViewModel(tagUsages);

		}

		public comments: EditableCommentsViewModel;

		private eventAssociationType = ko.observable<UserEventRelationshipType>(null);

		public hasEvent = ko.computed(() => {
			return !!this.eventAssociationType();
		});

		public isEventAttending = ko.computed(() => this.eventAssociationType() === UserEventRelationshipType.Attending);

		public isEventInterested = ko.computed(() => this.eventAssociationType() === UserEventRelationshipType.Interested);

		public removeEvent = () => {
			this.userRepo.deleteEventForUser(this.eventId);
			this.eventAssociationType(null);
			var link = _.find(this.usersAttending(), u => u.id === this.loggedUserId);
			this.usersAttending.remove(link);
		}

		public reportViewModel: ReportEntryViewModel;

		public setEventAttending = () => {
			this.userRepo.updateEventForUser(this.eventId, UserEventRelationshipType.Attending);
			this.eventAssociationType(UserEventRelationshipType.Attending);
			this.userRepo.getOne(this.loggedUserId, "MainPicture", user => {
				this.usersAttending.push(user);
			});
		}

		public setEventInterested = () => {
			this.userRepo.updateEventForUser(this.eventId, UserEventRelationshipType.Interested);
			this.eventAssociationType(UserEventRelationshipType.Interested);
			var link = _.find(this.usersAttending(), u => u.id === this.loggedUserId);
			this.usersAttending.remove(link);
		}

		public tagsEditViewModel: TagsEditViewModel;

		public tagUsages: TagListViewModel;

		public usersAttending: KnockoutObservableArray<UserBaseContract>;

	}

//}
