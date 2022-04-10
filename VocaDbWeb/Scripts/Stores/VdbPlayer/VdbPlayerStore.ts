import EntryContract from '@DataContracts/EntryContract';
import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import { action, computed, makeObservable, observable } from 'mobx';

import PlayQueueStore from './PlayQueueStore';

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
	public readonly playQueueStore = new PlayQueueStore();

	public constructor() {
		makeObservable(this);
	}

	@computed public get hasPreviousEntry(): boolean {
		return this.playQueueStore.hasPreviousEntry;
	}

	@computed public get hasNextEntry(): boolean {
		return this.playQueueStore.hasNextEntry;
	}

	@computed public get selectedEntry(): IVdbPlayerEntry | undefined {
		return this.playQueueStore.selectedEntry;
	}

	@computed public get canAutoplay(): boolean {
		return (
			!!this.selectedEntry &&
			VdbPlayerStore.autoplayServices.includes(
				PVService[this.selectedEntry.pv.service as keyof typeof PVService],
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
		this.playQueueStore.previous();
	};

	public next = (): void => {
		this.playQueueStore.next();
	};

	public play = (entry: IVdbPlayerEntry): void => {
		this.playQueueStore.play(entry);

		if (!this.canAutoplay) {
			this.expand();
		}
	};

	public playNext = (entry: IVdbPlayerEntry): void => {
		if (this.playQueueStore.isEmpty) {
			this.play(entry);
			return;
		}

		this.playQueueStore.playNext(entry);
	};

	public addToQueue = (entry: IVdbPlayerEntry): void => {
		if (this.playQueueStore.isEmpty) {
			this.play(entry);
			return;
		}

		this.playQueueStore.addToQueue(entry);
	};
}
