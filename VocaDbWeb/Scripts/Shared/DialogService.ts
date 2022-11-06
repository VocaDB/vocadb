export interface IDialogService {
	confirm(message: string): boolean;
}

export class DialogService implements IDialogService {
	confirm = (message: string): boolean => window.confirm(message);
}
