import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	EntryType,
	PlayQueueAlbumContract,
	PlayQueueEntryContract,
	PlayQueueItemContract,
	PlayQueueReleaseEventContract,
	PlayQueueRepository,
	PlayQueueRepositoryFactory,
	PlayQueueRepositoryQueryParams,
	PlayQueueRepositoryType,
	PlayQueueSongContract,
} from '@/Stores/VdbPlayer/PlayQueueRepository';
import { SkipListStore } from '@/Stores/VdbPlayer/SkipListStore';
import { LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import addFormats from 'ajv-formats';
import { pull } from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export enum RepeatMode {
	Off = 'Off',
	All = 'All',
	One = 'One',
}

export interface PlayQueueLocalStorageState {
	repeat?: RepeatMode;
	shuffle?: boolean;
	items?: PlayQueueItemContract[];
	currentIndex?: number;
	repositoryType?: PlayQueueRepositoryType;
	queryParams?: PlayQueueRepositoryQueryParams;
	totalCount?: number;
	page?: number;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });
addFormats(ajv);

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<PlayQueueLocalStorageState> = require('./PlayQueueLocalStorageState.schema');
const validate = ajv.compile(schema);

export class PlayQueueItem {
	private static nextId = 1;

	public readonly id: number;
	// Do not use the name `selected`. See: https://github.com/SortableJS/react-sortablejs/issues/243.
	@observable public isSelected = false;
	private startTime?: number;
	@observable public currentTime = 0;

	public constructor(
		public readonly entry: PlayQueueEntryContract,
		public readonly pvId: number,
		startTime?: number,
	) {
		makeObservable(this);

		this.id = PlayQueueItem.nextId++;
		this.startTime = startTime;
	}

	public static fromContract = ({
		entry,
		pvId,
	}: PlayQueueItemContract): PlayQueueItem => {
		return new PlayQueueItem(entry, pvId);
	};

	public get pv(): PVContract {
		return this.entry.pvs.find((pv) => pv.id === this.pvId)!;
	}

	public getAndClearStartTime = (): number | undefined => {
		const startTime = this.startTime;
		this.startTime = undefined;
		return startTime;
	};

	@action public setCurrentTime = (value: number): void => {
		this.currentTime = value;
	};

	public toContract = (): PlayQueueItemContract => {
		return { entry: this.entry, pvId: this.pvId };
	};
}

export class AutoplayContext<
	TQueryParams extends PlayQueueRepositoryQueryParams
> {
	public constructor(
		public readonly repositoryType: PlayQueueRepositoryType,
		public readonly queryParams: TQueryParams,
	) {}
}

export enum PlayMethod {
	ClearAndPlay,
	PlayNext,
	AddToPlayQueue,
}

export class PlayQueueStore
	implements LocalStorageStateStore<PlayQueueLocalStorageState> {
	@observable public items: PlayQueueItem[] = [];
	@observable public currentId?: number;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;

	private autoplayContext?: AutoplayContext<PlayQueueRepositoryQueryParams>;
	private readonly paging = new ServerSidePagingStore(30);

	public readonly skipList: SkipListStore;

	public constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		private readonly eventRepo: ReleaseEventRepository,
		private readonly songRepo: SongRepository,
		private readonly playQueueRepoFactory: PlayQueueRepositoryFactory,
		artistRepo: ArtistRepository,
		tagRepo: TagRepository,
	) {
		makeObservable(this);

		this.skipList = new SkipListStore(values, artistRepo, tagRepo);
	}

	@computed.struct public get localStorageState(): PlayQueueLocalStorageState {
		return {
			repeat: this.repeat,
			shuffle: this.shuffle,
			items: this.items.map((item) => item.toContract()),
			currentIndex: this.currentIndex,
			repositoryType: this.autoplayContext?.repositoryType,
			queryParams: this.autoplayContext?.queryParams,
			totalCount: this.paging.totalItems,
			page: this.paging.page,
		};
	}
	public set localStorageState(value: PlayQueueLocalStorageState) {
		this.repeat = value.repeat ?? RepeatMode.Off;
		this.shuffle = value.shuffle ?? false;
		this.items = value.items?.map(PlayQueueItem.fromContract) ?? [];
		this.currentIndex = value.currentIndex;
		this.autoplayContext =
			value.repositoryType && value.queryParams
				? new AutoplayContext(value.repositoryType, value.queryParams)
				: undefined;
		this.paging.totalItems = value.totalCount ?? 0;
		this.paging.page = value.page ?? 1;
	}

	public validateLocalStorageState = (
		localStorageState: any,
	): localStorageState is PlayQueueLocalStorageState => {
		return validate(localStorageState);
	};

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

	@computed public get currentTime(): number | undefined {
		return this.currentItem?.currentTime;
	}
	public set currentTime(value: number | undefined) {
		if (!this.currentItem || value === undefined) return;

		this.currentItem.currentTime = value;
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

	@computed private get selectedItemsOrAllItems(): PlayQueueItem[] {
		return this.selectedItems.length > 0 ? this.selectedItems : this.items;
	}

	@computed public get shouldSkipCurrentItem(): boolean {
		return (
			this.currentItem !== undefined &&
			this.skipList.includesAny(this.currentItem.entry)
		);
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

	public playSelectedItemsNext = (): void => {
		const items = this.selectedItemsOrAllItems;
		this.playNext(
			items.map((item) => PlayQueueItem.fromContract(item.toContract())),
		);

		this.unselectAll();
	};

	@action public addToPlayQueue = (items: PlayQueueItem[]): void => {
		if (this.isEmpty) {
			this.clearAndPlay(items);
			return;
		}

		this.items.push(...items);
	};

	public addSelectedItemsToPlayQueue = (): void => {
		const items = this.selectedItemsOrAllItems;
		this.addToPlayQueue(
			items.map((item) => PlayQueueItem.fromContract(item.toContract())),
		);

		this.unselectAll();
	};

	@action public removeFromPlayQueue = async (
		items: PlayQueueItem[],
	): Promise<void> => {
		// Note: We need to remove the current (if any) and other (previous and/or next) items separately,
		// so that the current index can be set properly even if the current item was removed.

		// Capture the current item.
		const { currentItem } = this;

		// First, remove items that are not equal to the current one.
		pull(this.items, ...items.filter((item) => item !== currentItem));

		// Capture the current index.
		const { currentIndex, isLastItem } = this;

		// Then, remove the current item if any.
		pull(
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

	public removeSelectedItemsFromPlayQueue = async (): Promise<void> => {
		await this.removeFromPlayQueue(this.selectedItemsOrAllItems);

		this.unselectAll();
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

	private static createItemsFromSongs = (
		songs: PlayQueueSongContract[],
		autoplay: boolean,
	): PlayQueueItem[] => {
		return songs
			.map((song) => {
				const primaryPV = PVHelper.primaryPV(song.pvs, autoplay);
				return primaryPV ? new PlayQueueItem(song, primaryPV.id) : undefined;
			})
			.filter((item) => !!item)
			.map((item) => item!);
	};

	private getAlbumAndSongs = async ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<{
		album: PlayQueueAlbumContract;
		songs: PlayQueueSongContract[];
	}> => {
		const album = await this.albumRepo.getOneWithComponents({
			id: id,
			lang: lang,
			fields: PlayQueueRepository.albumOptionalFields,
			songFields: PlayQueueRepository.songOptionalFields,
		});

		return {
			album: {
				entryType: EntryType.Album,
				id: album.id,
				name: album.name,
				status: album.status,
				additionalNames: album.additionalNames,
				urlThumb: album.mainPicture?.urlThumb ?? '',
				pvs: album.pvs ?? [],
				artistIds:
					album.artists
						?.filter(({ artist }) => artist !== undefined)
						.map((artist) => artist.artist!.id) ?? [],
				tagIds: album.tags?.map((tag) => tag.tag.id) ?? [],
				artistString: album.artistString,
			},
			songs:
				album.tracks
					?.filter(({ song }) => !!song)
					.map((track) => track.song!)
					.map((song) => ({
						entryType: EntryType.Song,
						id: song.id,
						name: song.name,
						status: song.status,
						additionalNames: song.additionalNames,
						urlThumb: song.mainPicture?.urlThumb ?? '',
						pvs: song.pvs ?? [],
						artistIds:
							song.artists
								?.filter(({ artist }) => artist !== undefined)
								.map((artist) => artist.artist!.id) ?? [],
						tagIds: song.tags?.map((tag) => tag.tag.id) ?? [],
						artistString: song.artistString,
						songType: song.songType,
					})) ?? [],
		};
	};

	private loadItemsFromAlbum = async (
		entry: EntryContract,
		pv?: PVContract,
	): Promise<PlayQueueItem[]> => {
		const { album, songs } = await this.getAlbumAndSongs({
			id: entry.id,
			lang: this.values.languagePreference,
		});

		const primaryPV = pv ?? PVHelper.primaryPV(album.pvs, false);
		return [
			...(primaryPV ? [new PlayQueueItem(album, primaryPV.id)] : []),
			...PlayQueueStore.createItemsFromSongs(songs, true),
		];
	};

	private getEvent = async ({
		id,
	}: {
		id: number;
	}): Promise<PlayQueueReleaseEventContract> => {
		const event = await this.eventRepo.getOne({
			id: id,
			fields: PlayQueueRepository.eventOptionalFields,
		});

		return {
			entryType: EntryType.ReleaseEvent,
			id: event.id,
			name: event.name,
			status: event.status,
			additionalNames: event.additionalNames ?? '',
			urlThumb: event.mainPicture?.urlThumb ?? '',
			pvs: event.pvs ?? [],
			artistIds:
				event.artists
					?.filter(({ artist }) => artist !== undefined)
					.map((artist) => artist.artist!.id) ?? [],
			tagIds: event.tags?.map((tag) => tag.tag.id) ?? [],
		};
	};

	private loadItemsFromEvent = async (
		entry: EntryContract,
		pv?: PVContract,
	): Promise<PlayQueueItem[]> => {
		const event = await this.getEvent({
			id: entry.id,
		});

		const primaryPV = pv ?? PVHelper.primaryPV(event.pvs, false);
		return primaryPV ? [new PlayQueueItem(event, primaryPV.id)] : [];
	};

	private getSong = async ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<PlayQueueSongContract> => {
		const song = await this.songRepo.getOneWithComponents({
			id: id,
			lang: lang,
			fields: PlayQueueRepository.songOptionalFields,
		});

		return {
			entryType: EntryType.Song,
			id: song.id,
			name: song.name,
			status: song.status,
			additionalNames: song.additionalNames,
			urlThumb: song.mainPicture?.urlThumb ?? '',
			pvs: song.pvs ?? [],
			artistIds:
				song.artists
					?.filter(({ artist }) => artist !== undefined)
					.map((artist) => artist.artist!.id) ?? [],
			tagIds: song.tags?.map((tag) => tag.tag.id) ?? [],
			artistString: song.artistString,
			songType: song.songType,
		};
	};

	private loadItemsFromSong = async (
		entry: EntryContract,
		pv?: PVContract,
	): Promise<PlayQueueItem[]> => {
		const song = await this.getSong({
			id: entry.id,
			lang: this.values.languagePreference,
		});

		const primaryPV = pv ?? PVHelper.primaryPV(song.pvs, false);
		return primaryPV ? [new PlayQueueItem(song, primaryPV.id)] : [];
	};

	private loadItems = async (
		entry: EntryContract,
		pv?: PVContract,
	): Promise<PlayQueueItem[]> => {
		switch (entry.entryType) {
			case EntryType[EntryType.Album]:
				return this.loadItemsFromAlbum(entry, pv);

			case EntryType[EntryType.ReleaseEvent]:
				return this.loadItemsFromEvent(entry, pv);

			case EntryType[EntryType.Song]:
				return this.loadItemsFromSong(entry, pv);

			default:
				throw new Error(`Unsupported entry type: ${entry.entryType}`);
		}
	};

	public loadItemsAndPlay = async (
		method: PlayMethod,
		entry: EntryContract,
		pv?: PVContract,
	): Promise<void> => {
		const items = await this.loadItems(entry, pv);

		this.play(method, items);
	};

	@action public previous = (): void => {
		if (this.currentIndex === undefined) return;

		if (!this.hasPreviousItem) return;

		this.currentIndex--;

		if (this.shouldSkipCurrentItem) {
			this.previous();
		}
	};

	private updateResults = async (getTotalCount: boolean): Promise<void> => {
		if (!this.autoplayContext) return;
		const { repositoryType, queryParams } = this.autoplayContext;

		const playQueueRepo = this.playQueueRepoFactory.create(repositoryType);
		const pagingProps = this.paging.getPagingProperties(getTotalCount);
		const { items: songs, totalCount } = await playQueueRepo.getSongs({
			lang: this.values.languagePreference,
			pagingProps: pagingProps,
			pvServices: VideoServiceHelper.autoplayServices,
			queryParams: queryParams,
		});

		const songItems = PlayQueueStore.createItemsFromSongs(songs, true);

		runInAction(() => {
			this.items.push(...songItems);

			if (getTotalCount) this.paging.totalItems = totalCount;
		});
	};

	private updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	private updateResultsWithTotalCount = (): Promise<void> => {
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

		if (
			this.currentItem &&
			this.shouldSkipCurrentItem &&
			this.skipList.removeFromPlayQueueOnSkip
		) {
			this.removeFromPlayQueue([this.currentItem]);
		} else {
			this.currentIndex++;
		}

		if (this.shouldSkipCurrentItem) {
			await this.next();
		}
	};

	@action public goToFirst = (): void => {
		if (this.currentIndex === undefined) return;

		this.currentIndex = 0;
	};

	@action public startAutoplay = async <
		TQueryParams extends PlayQueueRepositoryQueryParams
	>(
		autoplayContext: AutoplayContext<TQueryParams>,
	): Promise<void> => {
		this.clear();

		this.autoplayContext = autoplayContext;

		await this.updateResultsWithTotalCount();

		this.setCurrentItem(this.items[0]);

		if (this.shouldSkipCurrentItem) {
			await this.next();
		}
	};

	@action public switchPV = (pv: PVContract): void => {
		const { currentIndex } = this;
		if (currentIndex === undefined) return;

		const { entry, currentTime } = this.items[currentIndex];
		const newItem = new PlayQueueItem(entry, pv.id, currentTime);
		this.items[currentIndex] = newItem;
		this.currentId = newItem.id;
	};

	@action public toggleRepeat = (): void => {
		switch (this.repeat) {
			case RepeatMode.Off:
				this.repeat = RepeatMode.All;
				break;

			case RepeatMode.All:
				this.repeat = RepeatMode.One;
				break;

			case RepeatMode.One:
				this.repeat = RepeatMode.Off;
				break;
		}
	};

	@action public toggleShuffle = (): void => {
		this.shuffle = !this.shuffle;
	};
}
