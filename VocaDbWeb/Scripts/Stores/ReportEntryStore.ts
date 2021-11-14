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

export default class ReportEntryStore {
	@observable public dialogVisible = false;
	@observable public notes = '';
	@observable public reportType?: IEntryReportType;

	public constructor(
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
	@computed public get isValid(): boolean {
		return (
			!this.reportType || !this.reportType.notesRequired || this.notes !== ''
		);
	}

	public send = async (): Promise<void> => {
		await this.sendFunc(this.reportType!.id, this.notes);

		runInAction(() => {
			this.notes = '';
			this.dialogVisible = false;
		});
	};

	@action public show = (): void => {
		this.dialogVisible = true;
	};
}
