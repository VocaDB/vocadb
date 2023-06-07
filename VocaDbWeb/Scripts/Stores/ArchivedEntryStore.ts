import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable } from 'mobx';

import schema from './ArchivedEntryRouteParams.schema.json';

export interface ArchivedEntryRouteParams {
	comparedVersionId?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
export const validate = ajv.compile<ArchivedEntryRouteParams>(schema);

interface IEntryReportsRepository {
	createReport({
		entryId,
		reportType,
		notes,
		versionNumber,
	}: {
		entryId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): Promise<void>;
}

export class ArchivedEntryStore
	implements LocationStateStore<ArchivedEntryRouteParams> {
	@observable comparedVersionId?: number;
	readonly reportStore: ReportEntryStore;

	constructor(
		entryId: number,
		versionNumber: number,
		entryReportsRepo: IEntryReportsRepository,
	) {
		makeObservable(this);

		this.reportStore = new ReportEntryStore(
			(reportType, notes) =>
				entryReportsRepo.createReport({
					entryId: entryId,
					reportType: reportType,
					notes: notes,
					versionNumber: versionNumber,
				}),
			{ notesRequired: true, id: 'Other', name: undefined },
		);
	}

	@computed.struct get locationState(): ArchivedEntryRouteParams {
		return { comparedVersionId: this.comparedVersionId };
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
