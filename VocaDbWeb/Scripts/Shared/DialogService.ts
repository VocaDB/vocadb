
module vdb.ui_dialog {
	
	export interface IDialogService {
		
		confirm(message: string): boolean;

	}

	export class DialogService implements IDialogService {
		
		public confirm = (message: string) => confirm(message);

	}

}