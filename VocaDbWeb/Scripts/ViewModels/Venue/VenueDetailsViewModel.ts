import { IEntryReportType } from "../ReportEntryViewModel";
import ReportEntryViewModel from "../ReportEntryViewModel";
import ui from '../../Shared/MessagesTyped';
import VenueRepository from "../../Repositories/VenueRepository";

	export default class VenueDetailsViewModel {

		constructor(
			repo: VenueRepository,
			reportTypes: IEntryReportType[],
			public loggedUserId: number,
			venueId: number) {

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {
				repo.createReport(venueId, reportType, notes, null);
				ui.showSuccessMessage(vdb.resources.shared.reportSent);
			});

		}

		public reportViewModel: ReportEntryViewModel;

	}