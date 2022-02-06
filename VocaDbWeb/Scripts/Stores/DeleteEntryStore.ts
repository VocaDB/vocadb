import { action, computed, makeObservable, observable } from 'mobx';

export default class DeleteEntryStore {
	@observable public dialogVisible = false;
	@observable public notes = '';

	public constructor(
		private readonly deleteCallback: (notes: string) => Promise<void>,
		public readonly notesRequired = false,
	) {
		makeObservable(this);
	}

	@computed public get isValid(): boolean {
		return !this.notesRequired || !!this.notes;
	}

	@action public deleteEntry = (): Promise<void> => {
		this.dialogVisible = false;
		return this.deleteCallback(this.notes);
	};

	@action public show = (): void => {
		this.dialogVisible = true;
	};
}
