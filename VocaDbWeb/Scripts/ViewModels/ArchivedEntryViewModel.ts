
import ReportEntryViewModel from './ReportEntryViewModel';
import ui from '../Shared/MessagesTyped';

//module vdb.viewModels {

	export default class ArchivedEntryViewModel {
		
		constructor(entryId: number, versionNumber: number, private readonly repository: IEntryReportsRepository) {
			
			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(entryId, reportType, notes, versionNumber);

				ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

	export interface IEntryReportsRepository {
		createReport(entryId: number, reportType: string, notes: string, version?: number);
	}

//} 