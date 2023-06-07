import React from 'react';

import { ILogger } from './ILogger';
import { PlayerOptions } from './PlayerApi';
import { PlayerApiImpl } from './PlayerApiImpl';

// https://github.com/VocaDB/vocadb/blob/e147650a8f1f85c8fa865d0ab562126c278527ec/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts.
export class SoundCloudPlayerApi extends PlayerApiImpl<HTMLIFrameElement> {
	private readonly player: SC.SoundCloudWidget;

	constructor(
		logger: ILogger,
		playerElementRef: React.MutableRefObject<HTMLIFrameElement>,
		options: PlayerOptions | undefined,
	) {
		super(logger, playerElementRef, options);

		this.player = SC.Widget(this.playerElementRef.current);
	}

	private getDurationCore(): Promise<number> {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			this.player.getDuration(resolve);
		});
	}

	attach(id: string): Promise<void> {
		return new Promise((resolve, reject /* TODO: reject */) => {
			this.player.bind(SC.Widget.Events.READY, () => {
				this.player.bind(
					SC.Widget.Events.PLAY_PROGRESS,
					async (event) => {
						const duration = await this.getDurationCore();

						this.options?.onTimeUpdate?.({
							duration: duration / 1000,
							percent: event.currentPosition / duration,
							seconds: event.currentPosition / 1000,
						});
					},
				);
				this.player.bind(SC.Widget.Events.ERROR, (event) =>
					this.options?.onError?.(event),
				);
				this.player.bind(SC.Widget.Events.PLAY, () =>
					this.options?.onPlay?.(),
				);
				this.player.bind(SC.Widget.Events.PAUSE, () =>
					this.options?.onPause?.(),
				);
				this.player.bind(SC.Widget.Events.FINISH, () =>
					this.options?.onEnded?.(),
				);

				this.options?.onLoaded?.({ id: id });
				resolve();
			});
		});
	}

	async detach(): Promise<void> {
		this.player.unbind(SC.Widget.Events.READY);
		this.player.unbind(SC.Widget.Events.PLAY_PROGRESS);
		this.player.unbind(SC.Widget.Events.ERROR);
		this.player.unbind(SC.Widget.Events.PLAY);
		this.player.unbind(SC.Widget.Events.PAUSE);
		this.player.unbind(SC.Widget.Events.FINISH);
	}

	private static playerLoadAsync(
		player: SC.SoundCloudWidget,
		url: string,
		options: Omit<SC.SoundCloudLoadOptions, 'callback'>,
	): Promise<void> {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			player.load(url, { ...options, callback: resolve });
		});
	}

	async loadVideo(id: string): Promise<void> {
		await SoundCloudPlayerApi.playerLoadAsync(this.player, id, {
			auto_play: true,
		});

		this.options?.onLoaded?.({ id: id });
	}

	async play(): Promise<void> {
		this.player.play();
	}

	async pause(): Promise<void> {
		this.player.pause();
	}

	async setCurrentTime(seconds: number): Promise<void> {
		this.player.seekTo(seconds * 1000);
	}

	async setVolume(volume: number): Promise<void> {
		this.player.setVolume(volume * 100);
	}

	async setMuted(muted: boolean): Promise<void> {
		this.setVolume(muted ? 0 : 1 /* TODO */);
	}

	async getDuration(): Promise<number | undefined> {
		const duration = await this.getDurationCore();
		return duration / 1000;
	}

	private getCurrentTimeCore(): Promise<number> {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			this.player.getPosition(resolve);
		});
	}

	async getCurrentTime(): Promise<number | undefined> {
		const position = await this.getCurrentTimeCore();
		return position / 1000;
	}

	private getVolumeCore(): Promise<number> {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			this.player.getVolume(resolve);
		});
	}

	async getVolume(): Promise<number | undefined> {
		const volume = await this.getVolumeCore();
		return volume / 100;
	}
}
