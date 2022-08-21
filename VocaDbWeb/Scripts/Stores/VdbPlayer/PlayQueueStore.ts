import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import _ from 'lodash';
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
	@observable public selectedId?: number;

	public constructor() {
		makeObservable(this);
	}

	@computed public get isEmpty(): boolean {
		return this.items.length === 0;
	}

	@computed public get hasMultipleItems(): boolean {
		return this.items.length > 1;
	}

	@computed public get selectedIndex(): number | undefined {
		return this.selectedId !== undefined
			? this.items.findIndex((item) => item.id === this.selectedId)
			: undefined;
	}
	public set selectedIndex(value: number | undefined) {
		this.selectedId = value !== undefined ? this.items[value].id : undefined;
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
		return this.items.find((item) => item.id === this.selectedId);
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

	@action public playNext = (item: PlayQueueItem): void => {
		if (this.selectedIndex === undefined) return;

		this.items.splice(this.selectedIndex + 1, 0, item);
	};

	@action public play = (item: PlayQueueItem): void => {
		this.clear();
		// selectedId must be set before playNext is called.
		this.selectedId = item.id;
		this.playNext(item);
	};

	@action public addItem = (item: PlayQueueItem): void => {
		this.items.push(item);
	};

	@action public removeItem = (item: PlayQueueItem): void => {
		_.pull(this.items, item);
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
