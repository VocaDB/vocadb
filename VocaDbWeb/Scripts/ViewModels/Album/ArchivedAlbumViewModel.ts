import AlbumRepository from '@Repositories/AlbumRepository';
import ui from '@Shared/MessagesTyped';
import vdb from '@Shared/VdbStatic';

import ReportEntryViewModel from '../ReportEntryViewModel';

export default class ArchivedAlbumViewModel {
  constructor(
    albumId: number,
    versionNumber: number,
    private repository: AlbumRepository,
  ) {
    this.reportViewModel = new ReportEntryViewModel(
      null!,
      (reportType, notes) => {
        repository.createReport(albumId, reportType, notes, versionNumber);

        ui.showSuccessMessage(vdb.resources.shared.reportSent);
      },
      { notesRequired: true, id: 'Other', name: null! },
    );
  }

  public reportViewModel: ReportEntryViewModel;
}
