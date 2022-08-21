import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVService } from '@/Models/PVs/PVService';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
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

	@computed public get hasPreviousEntry(): boolean {
		return this.playQueue.hasPreviousEntry;
	}

	@computed public get hasNextEntry(): boolean {
		return this.playQueue.hasNextEntry;
	}

	@computed public get selectedEntry(): IVdbPlayerEntry | undefined {
		return this.playQueue.selectedEntry;
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

	public play = (entry: IVdbPlayerEntry): void => {
		this.playQueue.play(entry);

		if (!this.canAutoplay) {
			this.expand();
		}
	};

	public playNext = (entry: IVdbPlayerEntry): void => {
		if (this.playQueue.isEmpty) {
			this.play(entry);
			return;
		}

		this.playQueue.playNext(entry);
	};

	public addToQueue = (entry: IVdbPlayerEntry): void => {
		if (this.playQueue.isEmpty) {
			this.play(entry);
			return;
		}

		this.playQueue.addToQueue(entry);
	};
}
