import ArtistRepository from '@Repositories/ArtistRepository';
import ui from '@Shared/MessagesTyped';
import vdb from '@Shared/VdbStatic';

import ReportEntryViewModel from '../ReportEntryViewModel';

export default class ArchivedArtistViewModel {
	constructor(
		artistId: number,
		versionNumber: number,
		private repository: ArtistRepository,
	) {
		this.reportViewModel = new ReportEntryViewModel(
			null!,
			(reportType, notes) => {
				repository.createReport(artistId, reportType, notes, versionNumber);

				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			},
			{ notesRequired: true, id: 'Other', name: null! },
		);
	}

	public reportViewModel: ReportEntryViewModel;
}
