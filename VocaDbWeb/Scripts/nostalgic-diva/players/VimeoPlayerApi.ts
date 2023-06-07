import React from 'react';

import { ILogger } from './ILogger';
import { PlayerOptions } from './PlayerApi';
import { PlayerApiImpl } from './PlayerApiImpl';

// https://github.com/cookpete/react-player/blob/e3c324bc6845698179d065fa408db515c2296b4b/src/players/Vimeo.js
export class VimeoPlayerApi extends PlayerApiImpl<HTMLIFrameElement> {
	private readonly player: Vimeo.Player;

	constructor(
		logger: ILogger,
		playerElementRef: React.MutableRefObject<HTMLIFrameElement>,
		options: PlayerOptions | undefined,
	) {
		super(logger, playerElementRef, options);

		this.player = new Vimeo.Player(this.playerElementRef.current);
	}

	async attach(): Promise<void> {
		await this.player.ready();

		this.player.on('error', (data) => this.options?.onError?.(data));
		this.player.on('loaded', (event) =>
			this.options?.onLoaded?.({ id: event.id.toString() }),
		);
		this.player.on('play', () => this.options?.onPlay?.());
		this.player.on('pause', () => this.options?.onPause?.());
		this.player.on('ended', () => this.options?.onEnded?.());
		this.player.on('timeupdate', (data) => {
			this.options?.onTimeUpdate?.({
				duration: data.duration,
				percent: data.percent,
				seconds: data.seconds,
			});
		});
	}

	async detach(): Promise<void> {
		this.player.off('error');
		this.player.off('loaded');
		this.player.off('play');
		this.player.off('pause');
		this.player.off('ended');
		this.player.off('timeupdate');
	}

	async loadVideo(id: string): Promise<void> {
		await this.player.loadVideo(id);
	}

	async play(): Promise<void> {
		await this.player.play();
	}

	async pause(): Promise<void> {
		await this.player.pause();
	}

	async setCurrentTime(seconds: number): Promise<void> {
		await this.player.setCurrentTime(seconds);
	}

	async setVolume(fraction: number): Promise<void> {
		await this.player.setVolume(fraction);
	}

	async setMuted(muted: boolean): Promise<void> {
		await this.player.setMuted(muted);
	}

	async getDuration(): Promise<number | undefined> {
		return this.player.getDuration();
	}

	async getCurrentTime(): Promise<number | undefined> {
		return await this.player.getCurrentTime();
	}

	async getVolume(): Promise<number | undefined> {
		return await this.player.getVolume();
	}
}
