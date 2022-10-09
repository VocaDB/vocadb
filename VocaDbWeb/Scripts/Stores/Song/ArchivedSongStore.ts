import { SongRepository } from '@/Repositories/SongRepository';
import {
	ArchivedEntryRouteParams,
	validate,
} from '@/Stores/ArchivedEntryStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@vocadb/route-sphere';
import { computed, makeObservable, observable } from 'mobx';

export class ArchivedSongStore
	implements LocationStateStore<ArchivedEntryRouteParams> {
	@observable public comparedVersionId?: number;
	public readonly reportStore: ReportEntryStore;

	public constructor(
		songId: number,
		versionNumber: number,
		songRepo: SongRepository,
	) {
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
