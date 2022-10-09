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
	@observable public comparedVersionId?: number;
	public readonly reportStore: ReportEntryStore;

	public constructor(
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

	@computed.struct public get locationState(): ArchivedEntryRouteParams {
		return {
			comparedVersionId: this.comparedVersionId,
		};
	}
	public set locationState(value: ArchivedEntryRouteParams) {
		this.comparedVersionId = value.comparedVersionId;
	}

	public validateLocationState = (
		locationState: any,
	): locationState is ArchivedEntryRouteParams => {
		return validate(locationState);
	};
}
