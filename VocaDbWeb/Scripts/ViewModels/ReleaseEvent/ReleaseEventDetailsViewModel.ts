
namespace vdb.viewModels.releaseEvents {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ReleaseEventDetailsViewModel {

		constructor(urlMapper: vdb.UrlMapper,
			private userRepo: rep.UserRepository,
			latestComments: dc.CommentContract[],
			public loggedUserId: number,
			private eventId: number,
			eventAssociationType: models.users.UserEventRelationshipType,
			usersAttending: dc.UserBaseContract[],
			canDeleteAllComments: boolean) {

			const commentRepo = new rep.CommentRepository(urlMapper, vdb.models.EntryType.ReleaseEvent);
			this.comments = new EditableCommentsViewModel(commentRepo, eventId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);
			this.eventAssociationType(eventAssociationType);
			this.usersAttending = ko.observableArray(usersAttending);

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

		public usersAttending: KnockoutObservableArray<dc.UserBaseContract>;

	}

}
