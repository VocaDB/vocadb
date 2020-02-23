
namespace vdb.viewModels.venues {

	export class VenueDetailsViewModel {

		constructor(
			repo: rep.VenueRepository,
			reportTypes: IEntryReportType[],
			public loggedUserId: number,
			venueId: number) {

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {
				repo.createReport(venueId, reportType, notes, null);
				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);
			});

		}

		public reportViewModel: ReportEntryViewModel;

	}

}
