import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export interface PlayQueueItemContract {
	entry: EntryContract;
	pv: PVContract;
}

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

	public static fromContract = ({
		entry,
		pv,
	}: PlayQueueItemContract): PlayQueueItem => {
		return new PlayQueueItem(entry, pv);
	};

	public toContract = (): PlayQueueItemContract => {
		return { entry: this.entry, pv: this.pv };
	};
}

export enum PlayMethod {
	ClearAndPlay,
	PlayNext,
	AddToPlayQueue,
}

type AutoplayCallback<TQueryParams> = (
	pagingProps: PagingProperties,
	queryParams: TQueryParams,
) => Promise<PartialFindResultContract<PlayQueueItem>>;

export class AutoplayContext<TQueryParams> {
	public constructor(
		public readonly queryParams: TQueryParams,
		public readonly callback: AutoplayCallback<TQueryParams>,
	) {}
}

export class PlayQueueStore {
	@observable public items: PlayQueueItem[] = [];
	@observable public currentId?: number;

	private autoplayContext?: AutoplayContext<any>;
	private totalCount = 0;
	private start = 0;
	@observable public hasMoreItems = false;

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
		this.currentId =
			value !== undefined
				? this.items[value] /* TODO: Replace with this.items.at(value) */?.id
				: undefined;
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
			this.hasMoreItems ||
			(this.hasMultipleItems &&
				this.currentIndex !== undefined &&
				this.currentIndex < this.items.length - 1)
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

		this.autoplayContext = undefined;
		this.totalCount = 0;
		this.start = 0;
		this.hasMoreItems = false;
	};

	@action public unselectAll = (): void => {
		for (const item of this.items) {
			item.isSelected = false;
		}
	};

	@action public setCurrentItem = (item: PlayQueueItem | undefined): void => {
		this.currentId = item?.id;
	};

	@action private setNextItems = (items: PlayQueueItem[]): void => {
		if (this.currentIndex === undefined) return;

		this.items.splice(this.currentIndex + 1, 0, ...items);
	};

	@action public clearAndPlay = (items: PlayQueueItem[]): void => {
		this.clear();

		// currentId must be set before setNextItems is called.
		this.setCurrentItem(items[0]);

		this.setNextItems(items);
	};

	public playNext = (items: PlayQueueItem[]): void => {
		if (this.isEmpty) {
			this.clearAndPlay(items);
			return;
		}

		this.setNextItems(items);
	};

	@action public addToPlayQueue = (items: PlayQueueItem[]): void => {
		if (this.isEmpty) {
			this.clearAndPlay(items);
			return;
		}

		this.items.push(...items);
	};

	public play = (method: PlayMethod, items: PlayQueueItem[]): void => {
		switch (method) {
			case PlayMethod.ClearAndPlay:
				this.clearAndPlay(items);
				break;

			case PlayMethod.PlayNext:
				this.playNext(items);
				break;

			case PlayMethod.AddToPlayQueue:
				this.addToPlayQueue(items);
				break;
		}
	};

	@action public previous = (): void => {
		if (this.currentIndex === undefined) return;

		if (!this.hasPreviousItem) return;

		this.currentIndex--;
	};

	private loadMoreItems = async (getTotalCount: boolean): Promise<void> => {
		if (!this.autoplayContext) return;

		const { callback, queryParams } = this.autoplayContext;

		const pagingProps = {
			getTotalCount: getTotalCount,
			maxEntries: 30,
			start: this.start,
		};

		const { items, totalCount } = await callback(pagingProps, queryParams);

		if (getTotalCount) this.totalCount = totalCount;

		runInAction(() => {
			this.items.push(...items);
			this.start += pagingProps.maxEntries;
			this.hasMoreItems = this.start < this.totalCount;
		});
	};

	public loadMore = async (): Promise<void> => {
		if (!this.hasMoreItems) return;

		await this.loadMoreItems(false);
	};

	@action public next = async (): Promise<void> => {
		if (this.currentIndex === undefined) return;

		if (!this.hasNextItem) return;

		if (this.isLastItem) {
			await this.loadMore();
		}

		this.currentIndex++;
	};

	@action public goToFirst = (): void => {
		if (this.currentIndex === undefined) return;

		this.currentIndex = 0;
	};

	@action public removeFromPlayQueue = async (
		items: PlayQueueItem[],
	): Promise<void> => {
		// Note: We need to remove the current (if any) and other (previous and/or next) items separately,
		// so that the current index can be set properly even if the current item was removed.

		// Capture the current item.
		const { currentItem } = this;

		// First, remove items that are not equal to the current one.
		_.pull(this.items, ...items.filter((item) => item !== currentItem));

		// Capture the current index.
		const { currentIndex, isLastItem } = this;

		// Then, remove the current item if any.
		_.pull(
			this.items,
			items.find((item) => item === currentItem),
		);

		// If the current item differs from the captured one, then it means that the current item was removed from the play queue.
		if (this.currentItem !== currentItem) {
			if (isLastItem) {
				if (this.hasMoreItems) {
					await this.loadMore();

					// Set the current index to the captured one.
					this.currentIndex = currentIndex;
				} else {
					// Start over the playlist from the beginning.
					this.goToFirst();
				}
			} else {
				// Set the current index to the captured one.
				this.currentIndex = currentIndex;
			}
		}
	};

	@action public startAutoplay = async <TQueryParams>(
		autoplayContext: AutoplayContext<TQueryParams>,
	): Promise<void> => {
		this.clear();

		this.autoplayContext = autoplayContext;

		await this.loadMoreItems(true);

		this.setCurrentItem(this.items[0]);
	};
}
