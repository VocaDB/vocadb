import { PVService } from '@/Models/PVs/PVService';
import {
	PlayQueueItem,
	PlayQueueStore,
} from '@/Stores/VdbPlayer/PlayQueueStore';
import { action, computed, makeObservable, observable } from 'mobx';

export enum RepeatMode {
	Off = 'Off',
	All = 'All',
	One = 'One',
}

export class VdbPlayerStore {
	private static readonly autoplayServices = [
		PVService.File,
		PVService.LocalFile,
		PVService.NicoNicoDouga,
		PVService.Youtube,
		PVService.SoundCloud,
	];

	@observable public playing = false;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;
	@observable public expanded = false;
	public readonly playQueue = new PlayQueueStore();

	public constructor() {
		makeObservable(this);
	}

	@computed public get hasPreviousItem(): boolean {
		return this.playQueue.hasPreviousItem;
	}

	@computed public get hasNextItem(): boolean {
		return this.playQueue.hasNextItem;
	}

	@computed public get selectedItem(): PlayQueueItem | undefined {
		return this.playQueue.selectedItem;
	}

	@computed public get canAutoplay(): boolean {
		return (
			!!this.selectedItem &&
			VdbPlayerStore.autoplayServices.includes(
				PVService[this.selectedItem.pv.service as keyof typeof PVService],
			)
		);
	}

	@action public setPlaying = (value: boolean): void => {
		this.playing = value;
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

	@action public expand = (): void => {
		this.expanded = true;
	};

	@action public collapse = (): void => {
		this.expanded = false;
	};

	public previous = (): void => {
		this.playQueue.previous();
	};

	public next = (): void => {
		this.playQueue.next();
	};

	public goToFirst = (): void => {
		this.playQueue.goToFirst();
	};

	public play = (item: PlayQueueItem): void => {
		this.playQueue.play(item);
	};

	public clearAndPlay = (item: PlayQueueItem): void => {
		this.playQueue.clearAndPlay(item);

		if (!this.canAutoplay) {
			this.expand();
		}
	};

	public playNext = (item: PlayQueueItem): void => {
		if (this.playQueue.isEmpty) {
			this.clearAndPlay(item);
			return;
		}

		this.playQueue.playNext(item);
	};

	public addToQueue = (item: PlayQueueItem): void => {
		if (this.playQueue.isEmpty) {
			this.clearAndPlay(item);
			return;
		}

		this.playQueue.addItem(item);
	};

	public removeFromQueue = (item: PlayQueueItem): void => {
		if (this.selectedItem === item) {
			if (this.hasNextItem) {
				this.next();
			} else {
				this.goToFirst();
			}
		}

		this.playQueue.removeItem(item);
	};
}
