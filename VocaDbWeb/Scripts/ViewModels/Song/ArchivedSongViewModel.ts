
module vdb.viewModels.songs {

	import rep = repositories;

	export class ArchivedSongViewModel {
		
		constructor(songId: number, versionNumber: number, private repository: rep.SongRepository) {
			
			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(songId, reportType, notes, versionNumber);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

} 