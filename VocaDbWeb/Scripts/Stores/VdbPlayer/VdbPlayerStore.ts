import { getPiaproTimestamp } from '@/Components/Shared/Partials/PV/EmbedPiapro';
import { PVService } from '@/Models/PVs/PVService';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { action, computed, makeObservable, observable, reaction } from 'mobx';

export enum RepeatMode {
	Off = 'Off',
	All = 'All',
	One = 'One',
}

interface Rectangle {
	x: number;
	y: number;
	width: number;
	height: number;
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
	public readonly playQueue = new PlayQueueStore();
	@observable public playerBounds?: Rectangle;
	@observable public percent = 0;

	public constructor() {
		makeObservable(this);

		reaction(
			() => this.playQueue.currentItem,
			() => {
				this.percent = 0;
			},
		);
	}

	@computed public get canAutoplay(): boolean {
		const currentItem = this.playQueue.currentItem;

		if (!currentItem) return false;

		const { pv } = currentItem;

		if (pv.service === PVService[PVService.Piapro])
			return getPiaproTimestamp(pv) !== undefined;

		return VdbPlayerStore.autoplayServices.includes(
			PVService[pv.service as keyof typeof PVService],
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

	@action public setPlayerBounds = (value?: Rectangle): void => {
		this.playerBounds = value;
	};

	@action public setPercent = (value: number): void => {
		this.percent = value;
	};
}
