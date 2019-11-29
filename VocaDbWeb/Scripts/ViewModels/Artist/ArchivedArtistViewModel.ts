
import ArtistRepository from '../../Repositories/ArtistRepository';
import ReportEntryViewModel from '../ReportEntryViewModel';
import ui from '../../Shared/MessagesTyped';

//module vdb.viewModels.artists {

	export class ArchivedArtistViewModel {

		constructor(artistId: number, versionNumber: number, private repository: ArtistRepository) {

			this.reportViewModel = new ReportEntryViewModel(null, (reportType, notes) => {

				repository.createReport(artistId, reportType, notes, versionNumber);

				ui.showSuccessMessage(vdb.resources.shared.reportSent);

			}, { notesRequired: true, id: 'Other', name: null });

		}

		public reportViewModel: ReportEntryViewModel;

	}

//} 