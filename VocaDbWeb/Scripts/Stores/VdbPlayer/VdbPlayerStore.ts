import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import {
	PlayQueueRepositoryFactory,
	PlayQueueStore,
} from '@/Stores/VdbPlayer/PlayQueueStore';
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
	@observable public bottomBarEnabled = true;
	@observable public playing = false;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;
	public readonly playQueue: PlayQueueStore;
	@observable public playerBounds?: Rectangle;
	@observable public percent = 0;

	public constructor(playQueueRepoFactory: PlayQueueRepositoryFactory) {
		makeObservable(this);

		this.playQueue = new PlayQueueStore(playQueueRepoFactory);

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

		return VideoServiceHelper.canAutoplayPV(pv);
	}

	@action public showBottomBar = (): void => {
		this.bottomBarEnabled = true;
	};

	@action public hideBottomBar = (): void => {
		this.bottomBarEnabled = false;
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
