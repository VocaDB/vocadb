import React from 'react';

import { ILogger, LogLevel } from './ILogger';
import { PlayerOptions } from './PlayerApi';
import { PlayerApiImpl } from './PlayerApiImpl';

declare global {
	interface Window {
		onYouTubeIframeAPIReady(): void;
	}
}

enum PlayerState {
	UNSTARTED = -1,
	ENDED = 0,
	PLAYING = 1,
	PAUSED = 2,
	BUFFERING = 3,
	CUED = 5,
}

// https://github.com/VocaDB/vocadb/blob/076dac9f0808aba5da7332209fdfd2ff4e12c235/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerYoutube.ts.
export class YouTubePlayerApi extends PlayerApiImpl<HTMLDivElement> {
	private static readonly origin = 'https://www.youtube-nocookie.com';

	private readonly player: YT.Player;

	constructor(
		logger: ILogger,
		playerElementRef: React.MutableRefObject<HTMLDivElement>,
		options: PlayerOptions | undefined,
	) {
		super(logger, playerElementRef, options);

		this.player = new YT.Player(this.playerElementRef.current, {
			host: YouTubePlayerApi.origin,
			width: '100%',
			height: '100%',
		});
	}

	private previousTime?: number;

	private timeUpdateIntervalId?: number;

	private clearTimeUpdateInterval(): void {
		this.logger.log(
			LogLevel.Debug,
			'clearTimeUpdateInterval',
			this.timeUpdateIntervalId,
		);

		window.clearInterval(this.timeUpdateIntervalId);

		this.timeUpdateIntervalId = undefined;
	}

	private invokeTimeUpdate(player: YT.Player): void {
		const currentTime = player.getCurrentTime();
		if (currentTime === this.previousTime) return;

		const duration = player.getDuration();
		this.options?.onTimeUpdate?.({
			duration: duration,
			percent: currentTime / duration,
			seconds: currentTime,
		});

		this.previousTime = currentTime;
	}

	private setTimeUpdateInterval(): void {
		this.logger.log(LogLevel.Debug, 'setTimeUpdateInterval');

		this.clearTimeUpdateInterval();

		this.timeUpdateIntervalId = window.setInterval(
			() => this.invokeTimeUpdate(this.player),
			250,
		);

		this.logger.log(
			LogLevel.Debug,
			'timeUpdateIntervalId',
			this.timeUpdateIntervalId,
		);

		this.invokeTimeUpdate(this.player);
	}

	attach(id: string): Promise<void> {
		return new Promise((resolve, reject /* TODO: reject */) => {
			this.player.addEventListener('onReady', async () => {
				this.player.addEventListener('onError', (event) =>
					this.options?.onError?.(event.data),
				);
				this.player.addEventListener(
					'onStateChange',
					(event: YT.EventArgs): void => {
						this.logger.log(
							LogLevel.Debug,
							`state changed: ${PlayerState[event.data]}`,
						);

						switch (event.data) {
							case YT.PlayerState.CUED:
								this.options?.onLoaded?.({ id: id });
								break;

							case YT.PlayerState.PLAYING:
								this.options?.onPlay?.();
								this.setTimeUpdateInterval();
								break;

							case YT.PlayerState.PAUSED:
								this.options?.onPause?.();
								this.clearTimeUpdateInterval();
								break;

							case YT.PlayerState.ENDED:
								this.options?.onEnded?.();
								this.clearTimeUpdateInterval();
								break;
						}
					},
				);

				await this.loadVideo(id);
				resolve();
			});
		});
	}

	async detach(): Promise<void> {
		this.clearTimeUpdateInterval();
	}

	async loadVideo(id: string): Promise<void> {
		this.previousTime = undefined;
		this.player.cueVideoById(id);
	}

	async play(): Promise<void> {
		this.player.playVideo();
	}

	async pause(): Promise<void> {
		this.player.pauseVideo();
	}

	async setCurrentTime(seconds: number): Promise<void> {
		this.player.seekTo(seconds);

		this.invokeTimeUpdate(this.player);
	}

	async setVolume(volume: number): Promise<void> {
		this.player.setVolume(volume * 100);
	}

	async setMuted(muted: boolean): Promise<void> {
		if (muted) {
			this.player.mute();
		} else {
			this.player.unMute();
		}
	}

	async getDuration(): Promise<number | undefined> {
		return this.player.getDuration();
	}

	async getCurrentTime(): Promise<number | undefined> {
		return this.player.getCurrentTime();
	}

	async getVolume(): Promise<number | undefined> {
		return this.player.getVolume() / 100;
	}
}
