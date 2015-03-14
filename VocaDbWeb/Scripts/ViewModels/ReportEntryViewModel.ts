
module vdb.viewModels {
	
	export class ReportEntryViewModel {
		
		constructor(private sendFunc: (reportType: string, notes: string) => void) {}

		public dialogVisible = ko.observable(false);

		public notes = ko.observable("");

		public reportType = ko.observable<string>();

		public send = () => {			
			this.sendFunc(this.reportType(), this.notes());
			this.notes("");
			this.dialogVisible(false);
		}

		public show = () => {
			this.dialogVisible(true);
		}

	}

} 