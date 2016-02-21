
module vdb.viewModels {
	
	// Basic link to an entry with ID and name.
	// Allows changing the link by setting the ID.
	// Works well with LockingAutoComplete.
	export class BasicEntryLinkViewModel<TEntry extends vdb.models.IEntryWithIdAndName> {
		
		// entry: current entry reference (can be null).
		// entryFunc: function for loading the entry asynchronously by Id.
		constructor(entry: TEntry, entryFunc: (entryId: number, callback: (entry: TEntry) => void) => void) {

			this.entry = ko.observable(entry);
			this.name = ko.computed(() => this.entry() ? this.entry().name : null);

			this.id = ko.computed({
				read: () => (this.entry() ? this.entry().id : null),
				write: (entryId: number) => {

					// Get entry by ID or clear.
					if (entryId) {
						entryFunc(entryId, this.entry);						
					} else {
						this.entry(null);
					}

				}
			});

			this.isEmpty = ko.computed(() => this.entry() == null);

		}

		public entry: KnockoutObservable<TEntry>;

		public id: KnockoutComputed<number>;

		public isEmpty: KnockoutComputed<boolean>;

		public name: KnockoutComputed<string>;

	}

}