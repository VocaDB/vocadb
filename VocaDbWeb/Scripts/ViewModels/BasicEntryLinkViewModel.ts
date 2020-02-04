
import IEntryWithIdAndName from '../Models/IEntryWithIdAndName';

//module vdb.viewModels {
	
	// Basic link to an entry with ID and name.
	// Allows changing the link by setting the ID.
	// Works well with LockingAutoComplete.
	export default class BasicEntryLinkViewModel<TEntry extends IEntryWithIdAndName> {
		
		// entry: current entry reference (can be null). Zero-like ID will be considered the same as null.
		// entryFunc: function for loading the entry asynchronously by Id.
		constructor(entry?: TEntry, entryFunc?: (entryId: number, callback: (entry: TEntry) => void) => void) {

			this.entry = ko.observable(entry && entry.id ? entry : null);
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

		public clear = () => this.entry(null);

		public entry: KnockoutObservable<TEntry>;

		// Read/write entry ID. Both null and zero will clear the entry.
		public id: KnockoutComputed<number>;

		public isEmpty: KnockoutComputed<boolean>;

		public name: KnockoutComputed<string>;

		public subscribe = (callback: (newValue: number) => void) => this.id.subscribe(callback);

	}

//}