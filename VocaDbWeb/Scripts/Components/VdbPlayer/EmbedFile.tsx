import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';

// Code from: https://github.com/VocaDB/vocadb/blob/61b8c54f3eca906a477101dab4fdd9b154be310e/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerFile.ts.
class PVPlayerFile implements IPVPlayer {
	private player?: HTMLAudioElement;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLAudioElement>,
		private readonly options: IPVPlayerOptions,
	) {
		console.debug('[VdbPlayer] PVPlayerFile.ctor');
	}

	private attach = async (): Promise<void> => {
		if (this.player) {
			console.debug('[VdbPlayer] File player is already attached');
			return;
		}

		this.player = this.playerElementRef.current;

		console.debug('[VdbPlayer] File player attached');
	};

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerFile.load',
			JSON.parse(JSON.stringify(pv)),
		);

		if (!pv.url) {
			console.assert('[VdbPlayer] pv.url is not defined');
			return;
		}

		await this.attach();

		if (!this.player) {
			console.assert('[VdbPlayer] File player is not attached');
			return;
		}

		this.player.src = pv.url;

		// REVIEW: Do we need to remove event listeners before removing the player element?
		this.player.onplay = (): void => this.options.onPlay?.();
		this.player.onpause = (): void => this.options.onPause?.();
		this.player.onended = (): void => this.options.onEnded?.();
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerFile.play');

		if (!this.player) {
			console.assert('[VdbPlayer] File player is not attached');
			return;
		}

		this.player.play();
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerFile.pause');

		if (!this.player) {
			console.assert('[VdbPlayer] File player is not attached');
			return;
		}

		this.player.pause();
	};

	public seekTo = (seconds: number): void => {
		console.debug('[VdbPlayer] PVPlayerFile.seekTo');

		if (!this.player) {
			console.assert('[VdbPlayer] File player is not attached');
			return;
		}

		this.player.currentTime = seconds;
	};
}

interface EmbedFileProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedFile = React.memo(
	({ playerRef, ...options }: EmbedFileProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedFile');

		const playerElementRef = React.useRef<HTMLAudioElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerFile(playerElementRef, options);
		}, [playerRef, options]);

		return (
			<audio
				ref={playerElementRef}
				style={{ width: '100%', height: '100%' }}
				preload="auto"
				autoPlay
				controls
			/>
		);
	},
);

export default EmbedFile;
