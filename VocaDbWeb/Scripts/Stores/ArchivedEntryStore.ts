import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { LocationStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { computed, makeObservable, observable } from 'mobx';

export interface ArchivedEntryRouteParams {
	comparedVersionId?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<ArchivedEntryRouteParams> = require('./ArchivedEntryRouteParams.schema');
export const validate = ajv.compile(schema);

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
	@observable public comparedVersionId?: number;
	public readonly reportStore: ReportEntryStore;

	public constructor(
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

	@computed.struct public get locationState(): ArchivedEntryRouteParams {
		return { comparedVersionId: this.comparedVersionId };
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
