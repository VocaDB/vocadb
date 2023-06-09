import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { PlayQueueRepositoryFactory } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { LocalStorageStateStore } from '@/route-sphere';
import Ajv from 'ajv';
import { action, computed, makeObservable, observable, reaction } from 'mobx';

import schema from './VdbPlayerLocalStorageState.schema.json';

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
const validate = ajv.compile<VdbPlayerLocalStorageState>(schema);

export class VdbPlayerStore
	implements LocalStorageStateStore<VdbPlayerLocalStorageState> {
	@observable bottomBarEnabled = true;
	@observable playing = false;
	readonly playQueue: PlayQueueStore;
	@observable playerBounds?: Rectangle;
	@observable percent = 0;
	@observable songleWidgetEnabled = false;

	constructor(
		values: GlobalValues,
		albumRepo: AlbumRepository,
		eventRepo: ReleaseEventRepository,
		songRepo: SongRepository,
		playQueueRepoFactory: PlayQueueRepositoryFactory,
		artistRepo: ArtistRepository,
		tagRepo: TagRepository,
	) {
		makeObservable(this);

		this.playQueue = new PlayQueueStore(
			values,
			albumRepo,
			eventRepo,
			songRepo,
			playQueueRepoFactory,
			artistRepo,
			tagRepo,
		);

		reaction(
			() => this.playQueue.currentItem,
			() => {
				this.percent = 0;
			},
		);
	}

	@computed.struct get localStorageState(): VdbPlayerLocalStorageState {
		return {
			bottomBarEnabled: this.bottomBarEnabled,
		};
	}
	set localStorageState(value: VdbPlayerLocalStorageState) {
		this.bottomBarEnabled = value.bottomBarEnabled ?? true;
	}

	validateLocalStorageState = (
		localStorageState: any,
	): localStorageState is VdbPlayerLocalStorageState => {
		return validate(localStorageState);
	};

	@computed get canAutoplay(): boolean {
		const currentItem = this.playQueue.currentItem;
		if (!currentItem) return false;

		const { pv } = currentItem;
		return VideoServiceHelper.canAutoplayPV(pv);
	}

	@action showBottomBar = (): void => {
		this.bottomBarEnabled = true;
	};

	@action hideBottomBar = (): void => {
		this.bottomBarEnabled = false;
	};

	@action setPlaying = (value: boolean): void => {
		this.playing = value;
	};

	@action setPlayerBounds = (value?: Rectangle): void => {
		this.playerBounds = value;
	};

	@action setPercent = (value: number): void => {
		this.percent = value;
	};

	@action toggleSongleWidget = (): void => {
		this.songleWidgetEnabled = !this.songleWidgetEnabled;
	};
}
