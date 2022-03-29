import EntryContract from '@DataContracts/EntryContract';
import PVContract from '@DataContracts/PVs/PVContract';
import { action, computed, makeObservable, observable } from 'mobx';

export enum RepeatMode {
	Off = 'Off',
	All = 'All',
	One = 'One',
}

export interface IVdbPlayerEntry {
	entry: EntryContract;
	pv: PVContract;
}

export default class VdbPlayerStore {
	@observable public playing = false;
	@observable public repeat = RepeatMode.Off;
	@observable public shuffle = false;
	@observable public entry?: IVdbPlayerEntry;

	public constructor() {
		makeObservable(this);
	}

	@computed public get hasPreviousSong(): boolean {
		return false /* TODO: Implement. */;
	}

	@computed public get hasNextSong(): boolean {
		return false /* TODO: Implement. */;
	}

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

	@action public play = (): void => {
		this.playing = true;

		// TODO: Implement.
	};

	@action public pause = (): void => {
		this.playing = false;

		// TODO: Implement.
	};

	public previous = (): void => {
		// TODO: Implement.
	};

	public next = (): void => {
		// TODO: Implement.
	};

	@action public selectEntry = (entry: IVdbPlayerEntry): void => {
		this.entry = entry;

		this.play();
	};
}
