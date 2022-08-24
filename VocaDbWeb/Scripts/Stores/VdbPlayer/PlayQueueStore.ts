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
	@observable public currentId?: number;

	public constructor() {
		makeObservable(this);
	}

	@computed public get isEmpty(): boolean {
		return this.items.length === 0;
	}

	@computed public get hasMultipleItems(): boolean {
		return this.items.length > 1;
	}

	@computed public get currentIndex(): number | undefined {
		return this.currentId !== undefined
			? this.items.findIndex((item) => item.id === this.currentId)
			: undefined;
	}
	public set currentIndex(value: number | undefined) {
		this.currentId = value !== undefined ? this.items[value].id : undefined;
	}

	@computed public get hasPreviousItem(): boolean {
		return (
			this.hasMultipleItems &&
			this.currentIndex !== undefined &&
			this.currentIndex > 0
		);
	}

	@computed public get hasNextItem(): boolean {
		return (
			this.hasMultipleItems &&
			this.currentIndex !== undefined &&
			this.currentIndex < this.items.length - 1
		);
	}

	@computed public get currentItem(): PlayQueueItem | undefined {
		return this.items.find((item) => item.id === this.currentId);
	}

	@computed public get isLastItem(): boolean {
		return (
			this.currentIndex !== undefined &&
			this.currentIndex === this.items.length - 1
		);
	}

	@action public clear = (): void => {
		this.currentIndex = undefined;
		this.items = [];
	};

	@action public play = (item: PlayQueueItem): void => {
		this.currentId = item.id;
	};

	@action public playNext = (item: PlayQueueItem): void => {
		if (this.currentIndex === undefined) return;

		this.items.splice(this.currentIndex + 1, 0, item);
	};

	@action public clearAndPlay = (item: PlayQueueItem): void => {
		this.clear();
		// currentId must be set before playNext is called.
		this.play(item);
		this.playNext(item);
	};

	@action public addItem = (item: PlayQueueItem): void => {
		this.items.push(item);
	};

	@action public removeItem = (item: PlayQueueItem): void => {
		_.pull(this.items, item);
	};

	@action public previous = (): void => {
		if (this.currentIndex === undefined) return;

		if (!this.hasPreviousItem) return;

		this.currentIndex--;
	};

	@action public next = (): void => {
		if (this.currentIndex === undefined) return;

		if (!this.hasNextItem) return;

		this.currentIndex++;
	};

	@action public goToFirst = (): void => {
		if (this.currentIndex === undefined) return;

		this.currentIndex = 0;
	};
}
