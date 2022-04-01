import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';

// Code from: https://github.com/VocaDB/vocadb/blob/076dac9f0808aba5da7332209fdfd2ff4e12c235/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerYoutube.ts.
class PVPlayerYouTube implements IPVPlayer {
	private player?: YT.Player;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLDivElement>,
		private readonly options: IPVPlayerOptions,
	) {
		console.debug('[VdbPlayer] PVPlayerYouTube.ctor', playerElementRef.current);
	}

	private attach = (): Promise<void> => {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				console.debug('[VdbPlayer] YouTube player is already attached');

				resolve();
				return;
			}

			this.player = new YT.Player(this.playerElementRef.current, {
				width: '100%',
				height: '100%',
				events: {
					onReady: (): void => {
						console.debug('[VdbPlayer] YouTube player attached');

						resolve();
					},
					onError: (e): void => this.options.onError?.(e),
					onStateChange: (e: YT.EventArgs): void => {
						if (!this.player) {
							console.warn('[VdbPlayer] YouTube player is not attached');
							return;
						}

						switch (e.data) {
							case YT.PlayerState.PLAYING:
								console.debug('[VdbPlayer] YouTube state changed: PLAYING');

								this.options.onPlay?.();
								break;

							case YT.PlayerState.PAUSED:
								console.debug('[VdbPlayer] YouTube state changed: PAUSED');

								this.options.onPause?.();
								break;

							case YT.PlayerState.ENDED:
								console.debug('[VdbPlayer] YouTube state changed: ENDED');

								this.options.onEnded?.();
								break;
						}
					},
				},
			});
		});
	};

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerYouTube.load',
			JSON.parse(JSON.stringify(pv)),
		);

		console.debug('[VdbPlayer] Attaching YouTube player...');

		await this.attach();

		if (!this.player) {
			console.warn('[VdbPlayer] YouTube player is not attached');
			return;
		}

		console.debug('[VdbPlayer] Loading YouTube video...');

		this.player.loadVideoById(pv.pvId);
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerYouTube.play');

		if (!this.player) {
			console.warn('[VdbPlayer] YouTube player is not attached');
			return;
		}

		this.player.playVideo();
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerYouTube.pause');

		if (!this.player) {
			console.warn('[VdbPlayer] YouTube player is not attached');
			return;
		}

		this.player.pauseVideo();
	};

	public seekTo = (seconds: number): void => {
		console.debug('[VdbPlayer] PVPlayerYouTube.seekTo');

		if (!this.player) {
			console.warn('[VdbPlayer] YouTube player is not attached');
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
		console.debug('[VdbPlayer] EmbedYouTube');

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
