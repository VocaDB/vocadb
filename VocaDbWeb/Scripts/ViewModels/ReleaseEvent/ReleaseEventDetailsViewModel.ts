
namespace vdb.viewModels.releaseEvents {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ReleaseEventDetailsViewModel {

		constructor(
			urlMapper: vdb.UrlMapper,
			private readonly repo: rep.ReleaseEventRepository,
			private readonly userRepo: rep.UserRepository,
			latestComments: dc.CommentContract[],
			reportTypes: IEntryReportType[],
			public loggedUserId: number,
			private readonly eventId: number,
			eventAssociationType: models.users.UserEventRelationshipType,
			usersAttending: dc.UserBaseContract[],
			tagUsages: dc.tags.TagUsageForApiContract[],
			canDeleteAllComments: boolean) {

			const commentRepo = new rep.CommentRepository(urlMapper, vdb.models.EntryType.ReleaseEvent);
			this.comments = new EditableCommentsViewModel(commentRepo, eventId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);
			this.eventAssociationType(eventAssociationType);
			this.usersAttending = ko.observableArray(usersAttending);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {
				repo.createReport(eventId, reportType, notes, null);
				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);
			});

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepo.getEventTagSelections(this.eventId, callback),
				saveTagSelections: tags => userRepo.updateEventTags(this.eventId, tags, this.tagUsages.updateTagUsages)
			}, cls.EntryType.ReleaseEvent);

			this.tagUsages = new tags.TagListViewModel(tagUsages);

		}

		public comments: EditableCommentsViewModel;

		private eventAssociationType = ko.observable<models.users.UserEventRelationshipType>(null);

		public hasEvent = ko.computed(() => {
			return !!this.eventAssociationType();
		});

		public isEventAttending = ko.computed(() => this.eventAssociationType() === models.users.UserEventRelationshipType.Attending);

		public isEventInterested = ko.computed(() => this.eventAssociationType() === models.users.UserEventRelationshipType.Interested);

		public removeEvent = () => {
			this.userRepo.deleteEventForUser(this.eventId);
			this.eventAssociationType(null);
			var link = _.find(this.usersAttending(), u => u.id === this.loggedUserId);
			this.usersAttending.remove(link);
		}

		public reportViewModel: ReportEntryViewModel;

		public setEventAttending = () => {
			this.userRepo.updateEventForUser(this.eventId, vdb.models.users.UserEventRelationshipType.Attending);
			this.eventAssociationType(models.users.UserEventRelationshipType.Attending);
			this.userRepo.getOne(this.loggedUserId, "MainPicture", user => {
				this.usersAttending.push(user);
			});
		}

		public setEventInterested = () => {
			this.userRepo.updateEventForUser(this.eventId, vdb.models.users.UserEventRelationshipType.Interested);
			this.eventAssociationType(models.users.UserEventRelationshipType.Interested);
			var link = _.find(this.usersAttending(), u => u.id === this.loggedUserId);
			this.usersAttending.remove(link);
		}

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

		public usersAttending: KnockoutObservableArray<dc.UserBaseContract>;

	}

}
