
module vdb.viewModels {

	import rep = repositories;

	export class ArchivedEntryViewModel {
		
		constructor(entryId: number, versionNumber: number, private readonly repository: IEntryReportsRepository) {
			
			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(entryId, reportType, notes, versionNumber);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

	export interface IEntryReportsRepository {
		createReport(entryId: number, reportType: string, notes: string, version?: number);
	}

} 