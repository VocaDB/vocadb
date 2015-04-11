
module vdb.viewModels.artists {

	import rep = repositories;

	export class ArchivedArtistViewModel {

		constructor(artistId: number, versionNumber: number, private repository: rep.ArtistRepository) {

			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(artistId, reportType, notes, versionNumber);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

} 