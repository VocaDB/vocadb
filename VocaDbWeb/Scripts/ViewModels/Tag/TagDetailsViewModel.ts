
module vdb.viewModels.tags {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class TagDetailsViewModel {
		
		constructor(repo: rep.TagRepository,
			latestComments: dc.CommentContract[],
			reportTypes: IEntryReportType[],
			loggedUserId: number,
			tagId: number,
			canDeleteAllComments: boolean,
			showTranslatedDescription: boolean) {
			
			this.comments = new EditableCommentsViewModel(repo.getComments(), tagId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repo.createReport(tagId, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.description = new globalization.EnglishTranslatedStringViewModel(showTranslatedDescription);

		}

		public comments: EditableCommentsViewModel;

		public reportViewModel: ReportEntryViewModel;

		public description: globalization.EnglishTranslatedStringViewModel;

	}

}