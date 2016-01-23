
module vdb.viewModels.tags {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class TagDetailsViewModel {
		
		constructor(repo: rep.TagRepository,
			latestComments: dc.CommentContract[],
			loggedUserId: number,
			tagId: number,
			canDeleteAllComments: boolean,
			showTranslatedDescription: boolean) {
			
			this.comments = new EditableCommentsViewModel(repo.getComments(), tagId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);
			this.showTranslatedDescription = ko.observable(showTranslatedDescription);

		}

		public comments: EditableCommentsViewModel;

		public showTranslatedDescription: KnockoutObservable<boolean>;

	}

}