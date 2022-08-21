import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { action, computed, makeObservable, observable } from 'mobx';

export class PlayQueueItem {
	private static nextId = 1;

	public readonly id: number;

	public constructor(
		public readonly entry: EntryContract,
		public readonly pv: PVContract,
	) {
		this.id = PlayQueueItem.nextId++;
	}
}

export class PlayQueueStore {
	@observable public items: PlayQueueItem[] = [];
	@observable public selectedIndex?: number;

	public constructor() {
		makeObservable(this);
	}

	@computed public get isEmpty(): boolean {
		return this.items.length === 0;
	}

	@computed public get hasMultipleItems(): boolean {
		return this.items.length > 1;
	}

	@computed public get hasPreviousItem(): boolean {
		return (
			this.hasMultipleItems &&
			this.selectedIndex !== undefined &&
			this.selectedIndex > 0
		);
	}

	@computed public get hasNextItem(): boolean {
		return (
			this.hasMultipleItems &&
			this.selectedIndex !== undefined &&
			this.selectedIndex < this.items.length - 1
		);
	}

	@computed public get selectedItem(): PlayQueueItem | undefined {
		return this.selectedIndex !== undefined
			? this.items[this.selectedIndex]
			: undefined;
	}

	@computed public get isLastItem(): boolean {
		return (
			this.selectedIndex !== undefined &&
			this.selectedIndex === this.items.length - 1
		);
	}

	@action public clear = (): void => {
		this.selectedIndex = undefined;
		this.items = [];
	};

	@action public playNext = (entry: EntryContract, pv: PVContract): void => {
		if (this.selectedIndex === undefined) return;

		this.items.splice(this.selectedIndex + 1, 0, new PlayQueueItem(entry, pv));
	};

	@action public play = (entry: EntryContract, pv: PVContract): void => {
		this.clear();
		// selectedIndex must be set before playNext is called.
		this.selectedIndex = 0;
		this.playNext(entry, pv);
	};

	@action public addToQueue = (entry: EntryContract, pv: PVContract): void => {
		this.items.push(new PlayQueueItem(entry, pv));
	};

	@action public previous = (): void => {
		if (this.selectedIndex === undefined) return;

		if (!this.hasPreviousItem) return;

		this.selectedIndex--;
	};

	@action public next = (): void => {
		if (this.selectedIndex === undefined) return;

		if (!this.hasNextItem) return;

		this.selectedIndex++;
	};

	@action public goToFirst = (): void => {
		if (this.selectedIndex === undefined) return;

		this.selectedIndex = 0;
	};
}
