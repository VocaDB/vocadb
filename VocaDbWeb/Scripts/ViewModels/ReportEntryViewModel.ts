
module vdb.viewModels {
	
	export class ReportEntryViewModel {
		
		constructor(
			public reportTypes: IEntryReportType[],
			private sendFunc: (reportType: string, notes: string) => void,
			reportType?: IEntryReportType) { 
		
			this.isValid = ko.computed(() => !this.reportType() || !this.reportType().notesRequired || this.notes() !== "");
			this.reportType(reportType);
		
		}

		public dialogVisible = ko.observable(false);

		/** Report is valid to be sent (either notes are specified or not required) */
		public isValid: KnockoutComputed<boolean>;

		public notes = ko.observable("");

		public reportType = ko.observable<IEntryReportType>();

		public send = () => {			
			this.sendFunc(this.reportType().id, this.notes());
			this.notes("");
			this.dialogVisible(false);
		}

		public show = () => {
			this.dialogVisible(true);
		}

	}

	export interface IEntryReportType {

		// Report type ID
		id: string;

		// Localized name
		name: string;

		// Notes field is required for this report type
		notesRequired: boolean;

	}

} 