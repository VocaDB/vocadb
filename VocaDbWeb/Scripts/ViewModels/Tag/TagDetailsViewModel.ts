
//module vdb.viewModels.tags {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class TagDetailsViewModel {
		
		constructor(
			repo: rep.TagRepository,
			private userRepo: rep.UserRepository,
			latestComments: dc.CommentContract[],
			reportTypes: IEntryReportType[],
			loggedUserId: number,
			private tagId: number,
			canDeleteAllComments: boolean,
			showTranslatedDescription: boolean,
			isFollowed: boolean) {
			
			this.comments = new EditableCommentsViewModel(repo.getComments(), tagId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repo.createReport(tagId, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.description = new globalization.EnglishTranslatedStringViewModel(showTranslatedDescription);
			this.isFollowed = ko.observable(isFollowed);
			this.isLoggedIn = !!loggedUserId;

		}

		public comments: EditableCommentsViewModel;

		public followTag = () => {
			if (!this.isLoggedIn)
				return;
			this.userRepo.addFollowedTag(this.tagId);
			this.isFollowed(true);
		}

		public unfollowTag = () => {
			this.userRepo.deleteFollowedTag(this.tagId);
			this.isFollowed(false);			
		}

		public isFollowed: KnockoutObservable<boolean>;

		public isLoggedIn: boolean;

		public reportViewModel: ReportEntryViewModel;

		public description: globalization.EnglishTranslatedStringViewModel;

	}

//}