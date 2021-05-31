import ko from 'knockout';

export default class DeleteEntryViewModel {
	constructor(
		private deleteCallback: (notes: string) => void,
		public readonly notesRequired: boolean = false,
	) {}

	public deleteEntry = (): void => {
		this.dialogVisible(false);
		this.deleteCallback(this.notes());
	};

	public dialogVisible = ko.observable(false);

	public notes = ko.observable('');

	/** Report is valid to be sent (either notes are specified or not required) */
	public isValid = ko.computed(() => !this.notesRequired || this.notes());

	public show = (): void => this.dialogVisible(true);
}
