import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import {
	action,
	computed,
	IReactionDisposer,
	IReactionPublic,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

// Basic link to an entry with ID and name.
// Allows changing the link by setting the ID.
// Works well with LockingAutoComplete.
export default class BasicEntryLinkStore<TEntry extends IEntryWithIdAndName> {
	@observable public entry?: TEntry;

	// entry: current entry reference (can be null). Zero-like ID will be considered the same as null.
	// entryFunc: function for loading the entry asynchronously by Id.
	public constructor(
		entry?: TEntry,
		private readonly entryFunc?: (
			entryId: number,
		) => Promise<TEntry | undefined>,
	) {
		makeObservable(this);

		this.entry = entry && entry.id ? entry : undefined;
	}

	// Read/write entry ID. Both null and zero will clear the entry.
	@computed public get id(): number | undefined {
		return this.entry?.id;
	}
	public set id(value: number | undefined) {
		// Get entry by ID or clear.
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

	@computed public get isEmpty(): boolean {
		return !this.entry;
	}

	@computed public get name(): string | undefined {
		return this.entry?.name;
	}

	@action public clear = (): void => {
		this.entry = undefined;
	};

	public reaction = (
		effect: (
			arg: number | undefined,
			prev: number | undefined,
			r: IReactionPublic,
		) => void,
	): IReactionDisposer => {
		return reaction(() => this.id, effect);
	};
}
