import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';
import { computed, makeObservable, observable, runInAction } from 'mobx';

// Basic link to an entry with ID and name.
// Allows changing the link by setting the ID.
// Works well with LockingAutoComplete.
export class BasicEntryLinkStore<TEntry extends IEntryWithIdAndName> {
	@observable private _id?: number;
	@observable entry?: TEntry;

	// entry: current entry reference (can be null). Zero-like ID will be considered the same as null.
	// entryFunc: function for loading the entry asynchronously by Id.
	constructor(
		private readonly entryFunc?: (
			entryId: number,
		) => Promise<TEntry | undefined>,
	) {
		makeObservable(this);
	}

	@computed get id(): number | undefined {
		return this._id;
	}
	set id(value: number | undefined) {
		this._id = value;

		if (value) {
			this.entryFunc?.(value).then((entry) =>
				runInAction(() => {
					this.entry = entry;
				}),
			);
		} else {
			this.entry = undefined;
		}
	}

	@computed get name(): string | undefined {
		return this.entry?.name;
	}

	@computed get isEmpty(): boolean {
		return !this.id;
	}

	clear = (): void => {
		this.id = undefined;
	};
}
