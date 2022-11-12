import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export interface IEntryReportType {
	// Report type ID
	id: string;
	// Localized name
	name?: string;
	// Notes field is required for this report type
	notesRequired: boolean;
}

export class ReportEntryStore {
	@observable dialogVisible = false;
	@observable notes = '';
	@observable reportType?: IEntryReportType;

	constructor(
		private readonly sendFunc: (
			reportType: string,
			notes: string,
		) => Promise<void>,
		reportType?: IEntryReportType,
	) {
		makeObservable(this);

		this.reportType = reportType;
	}

	/** Report is valid to be sent (either notes are specified or not required) */
	@computed get isValid(): boolean {
		return (
			!this.reportType || !this.reportType.notesRequired || this.notes !== ''
		);
	}

	send = async (): Promise<void> => {
		await this.sendFunc(this.reportType!.id, this.notes);

		runInAction(() => {
			this.notes = '';
			this.dialogVisible = false;
		});
	};

	@action show = (): void => {
		this.dialogVisible = true;
	};
}
