import { ArtistRepository } from '@/Repositories/ArtistRepository';
import {
	ArchivedEntryRouteParams,
	validate,
} from '@/Stores/ArchivedEntryStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@vocadb/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

export class ArchivedArtistStore
	implements LocationStateStore<ArchivedEntryRouteParams> {
	@observable comparedVersionId?: number;
	readonly reportStore: ReportEntryStore;

	constructor(
		artistId: number,
		versionNumber: number,
		artistRepo: ArtistRepository,
	) {
		makeObservable(this);

		this.reportStore = new ReportEntryStore(
			(reportType, notes) =>
				artistRepo.createReport({
					artistId: artistId,
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
