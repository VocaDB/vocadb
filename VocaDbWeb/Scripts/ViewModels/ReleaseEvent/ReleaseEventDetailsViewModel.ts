
namespace vdb.viewModels.releaseEvents {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ReleaseEventDetailsViewModel {

		constructor(urlMapper: vdb.UrlMapper,
			latestComments: dc.CommentContract[],
			loggedUserId: number,
			eventId: number,
			canDeleteAllComments: boolean) {

			const commentRepo = new rep.CommentRepository(urlMapper, vdb.models.EntryType.ReleaseEvent);
			this.comments = new EditableCommentsViewModel(commentRepo, eventId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);

		}

		public comments: EditableCommentsViewModel;

	}

}
