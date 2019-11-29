
import AlbumRepository from '../../Repositories/AlbumRepository';
import ReportEntryViewModel from '../ReportEntryViewModel';
import ui from '../../Shared/MessagesTyped';

//module vdb.viewModels.albums {

	export class ArchivedAlbumViewModel {

		constructor(albumId: number, versionNumber: number, private repository: AlbumRepository) {

			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(albumId, reportType, notes, versionNumber);

				ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

//} 