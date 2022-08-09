import { VenueRepository } from '@/Repositories/VenueRepository';
import { ui } from '@/Shared/MessagesTyped';
import {
	IEntryReportType,
	ReportEntryViewModel,
} from '@/ViewModels/ReportEntryViewModel';

export class VenueDetailsViewModel {
	public constructor(
		repo: VenueRepository,
		reportTypes: IEntryReportType[],
		venueId: number,
	) {
		this.reportViewModel = new ReportEntryViewModel(
			reportTypes,
			(reportType, notes) => {
				repo.createReport({
					entryId: venueId,
					reportType: reportType,
					notes: notes,
					versionNumber: undefined,
				});
				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			},
		);
	}

	public reportViewModel: ReportEntryViewModel;
}
