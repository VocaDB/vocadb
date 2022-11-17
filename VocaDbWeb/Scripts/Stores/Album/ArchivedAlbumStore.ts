import { AlbumRepository } from '@/Repositories/AlbumRepository';
import type { ArchivedEntryRouteParams } from '@/Stores/ArchivedEntryStore';
import { validate } from '@/Stores/ArchivedEntryStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@vocadb/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

export class ArchivedAlbumStore
	implements LocationStateStore<ArchivedEntryRouteParams> {
	@observable comparedVersionId?: number;
	readonly reportStore: ReportEntryStore;

	constructor(
		albumId: number,
		versionNumber: number,
		albumRepo: AlbumRepository,
	) {
		makeObservable(this);

		this.reportStore = new ReportEntryStore(
			(reportType, notes) =>
				albumRepo.createReport({
					albumId: albumId,
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
