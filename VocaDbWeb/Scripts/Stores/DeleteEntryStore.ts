import { action, computed, makeObservable, observable } from 'mobx';

export class DeleteEntryStore {
	@observable dialogVisible = false;
	@observable notes = '';

	constructor(
		private readonly deleteCallback: (
			notes: string,
		) => Promise<void>,
		readonly notesRequired = false,
	) {
		makeObservable(this);
	}

	@computed get isValid(): boolean {
		return !this.notesRequired || !!this.notes;
	}

	@action deleteEntry = async (): Promise<void> => {
		this.dialogVisible = false;

		return this.deleteCallback(this.notes);
	};

	@action show = (): void => {
		this.dialogVisible = true;
	};
}
