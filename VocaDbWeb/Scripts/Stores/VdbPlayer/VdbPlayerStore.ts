import EntryContract from '@DataContracts/EntryContract';
import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
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
	@observable.struct public entry?: IVdbPlayerEntry;

	public constructor() {
		makeObservable(this);
	}

	@computed public get hasPreviousSong(): boolean {
		return false /* TODO: Implement. */;
	}

	@computed public get hasNextSong(): boolean {
		return false /* TODO: Implement. */;
	}

	@computed public get canAutoplay(): boolean {
		return (
			!!this.entry &&
			VdbPlayerStore.autoplayServices.includes(
				PVService[this.entry.pv.service as keyof typeof PVService],
			)
		);
	}

	@action public setPlaying = (value: boolean): void => {
		this.playing = value;
	};

	@action public toggleRepeat = (): void => {
		/*switch (this.repeat) {
			case RepeatMode.Off:
				this.repeat = RepeatMode.All;
				break;

			case RepeatMode.All:
				this.repeat = RepeatMode.One;
				break;

			case RepeatMode.One:
				this.repeat = RepeatMode.Off;
				break;
		}*/
		switch (this.repeat) {
			case RepeatMode.Off:
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
		// TODO: Implement.
	};

	public next = (): void => {
		// TODO: Implement.
	};

	// Starts playing if autoplay is available for this video service, or expands the player if not available.
	@action public selectEntry = (entry: IVdbPlayerEntry): void => {
		this.entry = entry;

		if (!this.canAutoplay) {
			this.expand();
		}
	};
}
