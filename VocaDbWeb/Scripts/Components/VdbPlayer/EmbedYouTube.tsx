import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';
import getScript from './getScript';

declare global {
	interface Window {
		onYouTubeIframeAPIReady: () => void;
	}
}

// Code from: https://github.com/VocaDB/vocadb/blob/076dac9f0808aba5da7332209fdfd2ff4e12c235/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerYoutube.ts.
class PVPlayerYouTube implements IPVPlayer {
	private player?: YT.Player;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLDivElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerYouTube.ctor', playerElementRef.current);
	}

	private static scriptLoaded = false;

	private loadScript = (): Promise<void> => {
		return new Promise(async (resolve, reject) => {
			if (PVPlayerYouTube.scriptLoaded) {
				VdbPlayerConsole.debug('YouTube script is already loaded');

				resolve();
				return;
			}

			// Code from: https://stackoverflow.com/a/18154942.
			window.onYouTubeIframeAPIReady = (): void => {
				VdbPlayerConsole.debug('YouTube iframe API ready');

				resolve();
			};

			try {
				VdbPlayerConsole.debug('Loading YouTube script...');

				await getScript('https://www.youtube.com/iframe_api');

				PVPlayerYouTube.scriptLoaded = true;

				VdbPlayerConsole.debug('YouTube script loaded');
			} catch {
				VdbPlayerConsole.error('Failed to load YouTube script');

				reject();
			}
		});
	};

	private assertPlayerAttached = (): void => {
		VdbPlayerConsole.assert(!!this.player, 'YouTube player is not attached');
	};

	private attach = (): Promise<void> => {
		return new Promise(async (resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				VdbPlayerConsole.debug('YouTube player is already attached');

				resolve();
				return;
			}

			await this.loadScript();

			VdbPlayerConsole.debug('Attaching YouTube player...');

			this.player = new YT.Player(this.playerElementRef.current, {
				host: 'https://www.youtube-nocookie.com',
				width: '100%',
				height: '100%',
				events: {
					onReady: (): void => {
						VdbPlayerConsole.debug('YouTube player attached');

						resolve();
					},
					onError: (e): void => this.options.onError?.(e),
					onStateChange: (e: YT.EventArgs): void => {
						this.assertPlayerAttached();
						if (!this.player) return;

						switch (e.data) {
							case YT.PlayerState.PLAYING:
								VdbPlayerConsole.debug('YouTube state changed: PLAYING');

								this.options.onPlay?.();
								break;

							case YT.PlayerState.PAUSED:
								VdbPlayerConsole.debug('YouTube state changed: PAUSED');

								this.options.onPause?.();
								break;

							case YT.PlayerState.ENDED:
								VdbPlayerConsole.debug('YouTube state changed: ENDED');

								this.options.onEnded?.();
								break;
						}
					},
				},
			});
		});
	};

	public load = async (pv: PVContract): Promise<void> => {
		VdbPlayerConsole.debug(
			'PVPlayerYouTube.load',
			JSON.parse(JSON.stringify(pv)),
		);

		await this.attach();

		this.assertPlayerAttached();
		if (!this.player) return;

		VdbPlayerConsole.debug('Loading YouTube video...');

		this.player.loadVideoById(pv.pvId);
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.play');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.playVideo();
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.pause');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.pauseVideo();
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.seekTo', seconds);

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.seekTo(seconds);
	};

	public mute = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.mute');

		this.assertPlayerAttached();
		if (!this.player) return;

		// TODO: Implement.
	};

	public unmute = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.unmute');

		this.assertPlayerAttached();
		if (!this.player) return;

		// TODO: Implement.
	};
}

interface EmbedYouTubeProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedYouTube = React.memo(
	({ playerRef, ...options }: EmbedYouTubeProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedYouTube');

		const playerElementRef = React.useRef<HTMLDivElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerYouTube(playerElementRef, options);
		}, [playerRef, options]);

		return (
			<div style={{ width: '100%', height: '100%' }}>
				<div ref={playerElementRef} />
			</div>
		);
	},
);

export default EmbedYouTube;
