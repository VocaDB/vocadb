import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';

// Code from: https://github.com/VocaDB/vocadb/blob/61b8c54f3eca906a477101dab4fdd9b154be310e/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerFile.ts.
class PVPlayerFile implements IPVPlayer {
	private player?: HTMLAudioElement;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLAudioElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerFile.ctor');
	}

	private attach = async (): Promise<void> => {
		if (this.player) {
			VdbPlayerConsole.debug('File player is already attached');
			return;
		}

		this.player = this.playerElementRef.current;

		VdbPlayerConsole.debug('File player attached');
	};

	private assertPlayerAttached = (): void => {
		VdbPlayerConsole.assert(!!this.player, 'File player is not attached');
	};

	public load = async (pv: PVContract): Promise<void> => {
		VdbPlayerConsole.debug('PVPlayerFile.load', JSON.parse(JSON.stringify(pv)));

		VdbPlayerConsole.assert(!!pv.url, 'pv.url is not defined');
		if (!pv.url) return;

		await this.attach();

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.src = pv.url;

		// REVIEW: Do we need to remove event listeners before removing the player element?
		this.player.onplay = (): void => this.options.onPlay?.();
		this.player.onpause = (): void => this.options.onPause?.();
		this.player.onended = (): void => this.options.onEnded?.();
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerFile.play');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.play();
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerFile.pause');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.pause();
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerFile.seekTo');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.currentTime = seconds;
	};

	public mute = (): void => {
		VdbPlayerConsole.debug('PVPlayerFile.mute');

		this.assertPlayerAttached();
		if (!this.player) return;

		// TODO: Implement.
	};

	public unmute = (): void => {
		VdbPlayerConsole.debug('PVPlayerFile.unmute');

		this.assertPlayerAttached();
		if (!this.player) return;

		// TODO: Implement.
	};
}

interface EmbedFileProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedFile = React.memo(
	({ playerRef, ...options }: EmbedFileProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedFile');

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
