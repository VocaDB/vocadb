import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';

// Code from: https://github.com/VocaDB/vocadb/blob/076dac9f0808aba5da7332209fdfd2ff4e12c235/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerYoutube.ts.
class PVPlayerYouTube implements IPVPlayer {
	private player?: YT.Player;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLDivElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerYouTube.ctor', playerElementRef.current);
	}

	private attach = (): Promise<void> => {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				VdbPlayerConsole.debug('YouTube player is already attached');

				resolve();
				return;
			}

			this.player = new YT.Player(this.playerElementRef.current, {
				width: '100%',
				height: '100%',
				events: {
					onReady: (): void => {
						VdbPlayerConsole.debug('YouTube player attached');

						resolve();
					},
					onError: (e): void => this.options.onError?.(e),
					onStateChange: (e: YT.EventArgs): void => {
						if (!this.player) {
							VdbPlayerConsole.warn('YouTube player is not attached');
							return;
						}

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

		VdbPlayerConsole.debug('Attaching YouTube player...');

		await this.attach();

		if (!this.player) {
			VdbPlayerConsole.warn('YouTube player is not attached');
			return;
		}

		VdbPlayerConsole.debug('Loading YouTube video...');

		this.player.loadVideoById(pv.pvId);
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.play');

		if (!this.player) {
			VdbPlayerConsole.warn('YouTube player is not attached');
			return;
		}

		this.player.playVideo();
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.pause');

		if (!this.player) {
			VdbPlayerConsole.warn('YouTube player is not attached');
			return;
		}

		this.player.pauseVideo();
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerYouTube.seekTo');

		if (!this.player) {
			VdbPlayerConsole.warn('YouTube player is not attached');
			return;
		}

		this.player.seekTo(seconds, false);
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
