
//module vdb.viewModels {
	
	export class DeleteEntryViewModel {
		
		constructor(private deleteCallback: (notes: string) => void) {}

		public deleteEntry = () => {
			this.dialogVisible(false);
			this.deleteCallback(this.notes());
		}

		public dialogVisible = ko.observable(false);

		public notes = ko.observable("");

		public show = () => this.dialogVisible(true);

	}

//} 