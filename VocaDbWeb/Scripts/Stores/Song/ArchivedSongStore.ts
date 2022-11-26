import { SongRepository } from '@/Repositories/SongRepository';
import type { ArchivedEntryRouteParams } from '@/Stores/ArchivedEntryStore';
import { validate } from '@/Stores/ArchivedEntryStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@vocadb/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

export class ArchivedSongStore
	implements LocationStateStore<ArchivedEntryRouteParams> {
	@observable comparedVersionId?: number;
	readonly reportStore: ReportEntryStore;

	constructor(songId: number, versionNumber: number, songRepo: SongRepository) {
		makeObservable(this);

		this.reportStore = new ReportEntryStore(
			(reportType, notes) =>
				songRepo.createReport({
					songId: songId,
					reportType: reportType,
					notes: notes,
					versionNumber: versionNumber,
				}),
			{ notesRequired: true, id: 'Other', name: undefined },
		);
	}

	@computed.struct get locationState(): ArchivedEntryRouteParams {
		return {
			comparedVersionId: this.comparedVersionId,
		};
	}
	set locationState(value: ArchivedEntryRouteParams) {
		this.comparedVersionId = value.comparedVersionId;
	}

	validateLocationState = (
		locationState: any,
	): locationState is ArchivedEntryRouteParams => {
		return validate(locationState);
	};
}
