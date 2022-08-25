import { PVService } from '@/Models/PVs/PVService';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
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

	@observable public bottomBarVisible = true;
	@observable public playing = false;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;
	@observable public expanded = false;
	public readonly playQueue = new PlayQueueStore();

	public constructor() {
		makeObservable(this);
	}

	@computed public get canAutoplay(): boolean {
		return (
			!!this.playQueue.currentItem &&
			VdbPlayerStore.autoplayServices.includes(
				PVService[
					this.playQueue.currentItem.pv.service as keyof typeof PVService
				],
			)
		);
	}

	@action public showBottomBar = (): void => {
		this.bottomBarVisible = true;
	};

	@action public hideBottomBar = (): void => {
		this.bottomBarVisible = false;
	};

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
}
