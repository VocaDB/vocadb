import VenueRepository from '@Repositories/VenueRepository';
import ui from '@Shared/MessagesTyped';

import { IEntryReportType } from '../ReportEntryViewModel';
import ReportEntryViewModel from '../ReportEntryViewModel';

export default class VenueDetailsViewModel {
  constructor(
    repo: VenueRepository,
    reportTypes: IEntryReportType[],
    public loggedUserId: number,
    venueId: number,
  ) {
    this.reportViewModel = new ReportEntryViewModel(
      reportTypes,
      (reportType, notes) => {
        repo.createReport(venueId, reportType, notes, null!);
        ui.showSuccessMessage(vdb.resources.shared.reportSent);
      },
    );
  }

  public reportViewModel: ReportEntryViewModel;
}
