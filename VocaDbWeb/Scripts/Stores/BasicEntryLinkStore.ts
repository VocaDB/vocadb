import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

// Basic link to an entry with ID and name.
// Allows changing the link by setting the ID.
// Works well with LockingAutoComplete.
export default class BasicEntryLinkStore<TEntry extends IEntryWithIdAndName> {
	@observable public id?: number;
	@observable public name?: string;

	// entry: current entry reference (can be null). Zero-like ID will be considered the same as null.
	// entryFunc: function for loading the entry asynchronously by Id.
	public constructor(
		private readonly entryFunc?: (
			entryId: number,
		) => Promise<TEntry | undefined>,
	) {
		makeObservable(this);
	}

	@computed public get entry(): TEntry | undefined {
		return this.id ? ({ id: this.id, name: this.name } as TEntry) : undefined;
	}

	@computed public get isEmpty(): boolean {
		return !this.entry;
	}

	@action public selectEntry = (entryId?: number): void => {
		this.id = entryId;

		if (entryId) {
			this.entryFunc?.(entryId).then((entry) =>
				runInAction(() => {
					this.name = entry?.name;
				}),
			);
		} else {
			this.name = undefined;
		}
	};

	public clear = (): void => {
		this.selectEntry(undefined);
	};
}
