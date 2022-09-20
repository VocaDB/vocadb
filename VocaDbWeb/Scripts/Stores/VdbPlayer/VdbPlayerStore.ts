import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import {
	PlayQueueRepositoryFactory,
	PlayQueueStore,
} from '@/Stores/VdbPlayer/PlayQueueStore';
import { LocalStorageStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
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

interface VdbPlayerLocalStorageState {
	bottomBarEnabled?: boolean;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<VdbPlayerLocalStorageState> = require('./VdbPlayerLocalStorageState.schema');
const validate = ajv.compile(schema);

export class VdbPlayerStore
	implements LocalStorageStateStore<VdbPlayerLocalStorageState> {
	@observable public bottomBarEnabled = true;
	@observable public playing = false;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;
	public readonly playQueue: PlayQueueStore;
	@observable public playerBounds?: Rectangle;
	@observable public percent = 0;

	public constructor(
		values: GlobalValues,
		albumRepo: AlbumRepository,
		eventRepo: ReleaseEventRepository,
		songRepo: SongRepository,
		playQueueRepoFactory: PlayQueueRepositoryFactory,
	) {
		makeObservable(this);

		this.playQueue = new PlayQueueStore(
			values,
			albumRepo,
			eventRepo,
			songRepo,
			playQueueRepoFactory,
		);

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

	@computed.struct public get localStorageState(): VdbPlayerLocalStorageState {
		return {
			bottomBarEnabled: this.bottomBarEnabled,
		};
	}
	public set localStorageState(value: VdbPlayerLocalStorageState) {
		this.bottomBarEnabled = value.bottomBarEnabled ?? true;
	}

	public validateLocalStorageState = (
		localStorageState: any,
	): localStorageState is VdbPlayerLocalStorageState => {
		return validate(localStorageState);
	};
}
