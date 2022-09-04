import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import _ from 'lodash';
import { action, computed, makeObservable, observable } from 'mobx';

export class PlayQueueItem {
	private static nextId = 1;

	public readonly id: number;
	// Do not use the name `selected`. See: https://github.com/SortableJS/react-sortablejs/issues/243.
	@observable public isSelected = false;

	public constructor(
		public readonly entry: EntryContract,
		public readonly pv: PVContract,
	) {
		makeObservable(this);

		this.id = PlayQueueItem.nextId++;
	}
}

export enum PlayMethod {
	ClearAndPlay,
	PlayNext,
	AddToPlayQueue,
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

	@computed public get selectedItems(): PlayQueueItem[] {
		return this.items.filter((item) => item.isSelected);
	}

	@computed public get allItemsSelected(): boolean {
		return this.selectedItems.length === this.items.length;
	}
	public set allItemsSelected(value: boolean) {
		for (const item of this.items) {
			item.isSelected = value;
		}
	}

	@action public clear = (): void => {
		this.currentIndex = undefined;
		this.items = [];
	};

	@action public unselectAll = (): void => {
		for (const item of this.items) {
			item.isSelected = false;
		}
	};

	@action public setCurrentItem = (item: PlayQueueItem): void => {
		this.currentId = item.id;
	};

	@action private setNextItems = (...items: PlayQueueItem[]): void => {
		if (this.currentIndex === undefined) return;

		this.items.splice(this.currentIndex + 1, 0, ...items);
	};

	@action public clearAndPlay = (...items: PlayQueueItem[]): void => {
		this.clear();
		// currentId must be set before setNextItems is called.
		this.setCurrentItem(items[0]);
		this.setNextItems(...items);
	};

	public playNext = (...items: PlayQueueItem[]): void => {
		if (this.isEmpty) {
			this.clearAndPlay(...items);
			return;
		}

		this.setNextItems(...items);
	};

	public addToPlayQueue = (...items: PlayQueueItem[]): void => {
		if (this.isEmpty) {
			this.clearAndPlay(...items);
			return;
		}

		this.items.push(...items);
	};

	public play = (method: PlayMethod, ...items: PlayQueueItem[]): void => {
		switch (method) {
			case PlayMethod.ClearAndPlay:
				this.clearAndPlay(...items);
				break;

			case PlayMethod.PlayNext:
				this.playNext(...items);
				break;

			case PlayMethod.AddToPlayQueue:
				this.addToPlayQueue(...items);
				break;
		}
	};

	public removeFromPlayQueue = (...items: PlayQueueItem[]): void => {
		for (const item of items) {
			if (this.currentItem === item) {
				if (this.hasNextItem) {
					this.next();
				} else {
					this.goToFirst();
				}
			}

			_.pull(this.items, item);
		}
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
