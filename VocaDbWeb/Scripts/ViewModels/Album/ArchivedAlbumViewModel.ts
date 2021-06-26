import AlbumRepository from '@Repositories/AlbumRepository';
import ui from '@Shared/MessagesTyped';

import ReportEntryViewModel from '../ReportEntryViewModel';

export default class ArchivedAlbumViewModel {
	public constructor(
		albumId: number,
		versionNumber: number,
		private repository: AlbumRepository,
	) {
		this.reportViewModel = new ReportEntryViewModel(
			null!,
			(reportType, notes) => {
				repository.createReport({
					albumId: albumId,
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
