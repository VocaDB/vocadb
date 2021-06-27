import ui from '@Shared/MessagesTyped';

import ReportEntryViewModel from './ReportEntryViewModel';

export default class ArchivedEntryViewModel {
	public constructor(
		entryId: number,
		versionNumber: number,
		private readonly repository: IEntryReportsRepository,
	) {
		this.reportViewModel = new ReportEntryViewModel(
			null!,
			(reportType, notes) => {
				repository.createReport({
					entryId: entryId,
					reportType: reportType,
					notes: notes,
					versionNumber: versionNumber,
				});

				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			},
			{ notesRequired: true, id: 'Other', name: null! },
		);
	}

	public reportViewModel: ReportEntryViewModel;
}

export interface IEntryReportsRepository {
	createReport({
		entryId,
		reportType,
		notes,
		versionNumber,
	}: {
		entryId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): void;
}
