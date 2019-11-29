
//module vdb.viewModels.albums {

	import rep = repositories;

	export class ArchivedAlbumViewModel {

		constructor(albumId: number, versionNumber: number, private repository: rep.AlbumRepository) {

			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(albumId, reportType, notes, versionNumber);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

//} 