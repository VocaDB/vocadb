import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueRepositoryForRatedSongsAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForRatedSongsAdapter';
import { PlayQueueRepositoryForSongListAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForSongListAdapter';
import { PlayQueueRepositoryForSongsAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForSongsAdapter';
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

export enum PlayQueueRepositoryType {
	RatedSongs = 'RatedSongs',
	SongList = 'SongList',
	Songs = 'Songs',
}

export class PlayQueueRepositoryFactory {
	public constructor(
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
	) {}

	public create = (type: PlayQueueRepositoryType): PlayQueueRepository<any> => {
		switch (type) {
			case PlayQueueRepositoryType.RatedSongs:
				return new PlayQueueRepositoryForRatedSongsAdapter(this.userRepo);

			case PlayQueueRepositoryType.SongList:
				return new PlayQueueRepositoryForSongListAdapter(this.songListRepo);

			case PlayQueueRepositoryType.Songs:
				return new PlayQueueRepositoryForSongsAdapter(this.songRepo);
		}
	};
}

export class AutoplayContext<TQueryParams> {
	public constructor(
		public readonly repositoryType: PlayQueueRepositoryType,
		public readonly queryParams: TQueryParams,
	) {}
}

export class PlayQueueStore {
	@observable public items: PlayQueueItem[] = [];
	@observable public currentId?: number;

	private autoplayContext?: AutoplayContext<any>;
	private readonly paging = new ServerSidePagingStore(30);

	public constructor(
		private readonly playQueueRepoFactory: PlayQueueRepositoryFactory,
	) {
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

	@computed public get hasMoreItems(): boolean {
		return !this.paging.isLastPage;
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

	@computed public get totalCount(): number {
		return this.paging.totalItems;
	}
	public set totalCount(value: number) {
		this.paging.totalItems = value;
	}

	@computed public get page(): number {
		return this.paging.page;
	}
	public set page(value: number) {
		this.paging.page = value;
	}

	@action public clear = (): void => {
		this.currentIndex = undefined;
		this.items = [];

		this.autoplayContext = undefined;
		this.paging.page = 1;
		this.paging.totalItems = 0;
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

	private updateResults = async (getTotalCount: boolean): Promise<void> => {
		if (!this.autoplayContext) return;

		const { repositoryType, queryParams } = this.autoplayContext;

		const playQueueRepo = this.playQueueRepoFactory.create(repositoryType);

		const pagingProps = this.paging.getPagingProperties(getTotalCount);

		const { items, totalCount } = await playQueueRepo.getItems(
			VideoServiceHelper.autoplayServices,
			pagingProps,
			queryParams,
		);

		runInAction(() => {
			this.items.push(...items);

			if (getTotalCount) this.paging.totalItems = totalCount;
		});
	};

	public updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	public updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	public loadMore = async (): Promise<void> => {
		if (!this.hasMoreItems) return;

		this.paging.nextPage();

		await this.updateResultsWithoutTotalCount();
	};

	@action public next = async (): Promise<void> => {
		if (this.currentIndex === undefined) return;

		if (!this.hasNextItem) return;

		if (this.isLastItem && this.hasMoreItems) {
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

		await this.updateResultsWithTotalCount();

		this.setCurrentItem(this.items[0]);
	};
}
