import { action, computed, makeObservable, observable } from 'mobx';

import { IVdbPlayerEntry } from './VdbPlayerStore';

export default class PlayQueueStore {
	@observable public queue: IVdbPlayerEntry[] = [];
	@observable public selectedIndex?: number;

	public constructor() {
		makeObservable(this);
	}

	@computed public get isEmpty(): boolean {
		return this.queue.length === 0;
	}

	@computed private get hasMultipleEntries(): boolean {
		return this.queue.length > 1;
	}

	@computed public get hasPreviousEntry(): boolean {
		return (
			this.hasMultipleEntries &&
			this.selectedIndex !== undefined &&
			this.selectedIndex > 0
		);
	}

	@computed public get hasNextEntry(): boolean {
		return (
			this.hasMultipleEntries &&
			this.selectedIndex !== undefined &&
			this.selectedIndex < this.queue.length - 1
		);
	}

	@computed public get selectedEntry(): IVdbPlayerEntry | undefined {
		return this.selectedIndex !== undefined
			? this.queue[this.selectedIndex]
			: undefined;
	}

	@action public clear = (): void => {
		this.selectedIndex = undefined;
		this.queue = [];
	};

	@action public playNext = (entry: IVdbPlayerEntry): void => {
		if (this.selectedIndex === undefined) return;

		this.queue.splice(this.selectedIndex + 1, 0, entry);
	};

	@action public play = (entry: IVdbPlayerEntry): void => {
		this.clear();
		// selectedIndex must be set before playNext is called.
		this.selectedIndex = 0;
		this.playNext(entry);
	};

	@action public addToQueue = (entry: IVdbPlayerEntry): void => {
		this.queue.push(entry);
	};

	@action public previous = (): void => {
		if (this.selectedIndex === undefined) return;

		if (!this.hasPreviousEntry) return;

		this.selectedIndex--;
	};

	@action public next = (): void => {
		if (this.selectedIndex === undefined) return;

		if (!this.hasNextEntry) return;

		this.selectedIndex++;
	};
}
