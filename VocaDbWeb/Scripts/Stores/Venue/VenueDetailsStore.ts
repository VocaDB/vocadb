import { VenueRepository } from '@/Repositories/VenueRepository';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';

export class VenueDetailsStore {
	readonly reportStore: ReportEntryStore;

	constructor(venueRepo: VenueRepository, venueId: number) {
		this.reportStore = new ReportEntryStore((reportType, notes) => {
			return venueRepo.createReport({
				entryId: venueId,
				reportType: reportType,
				notes: notes,
				versionNumber: undefined,
			});
		});
	}
}
