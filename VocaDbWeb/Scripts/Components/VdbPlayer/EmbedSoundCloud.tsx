import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';

// Code from: https://github.com/VocaDB/vocadb/blob/e147650a8f1f85c8fa865d0ab562126c278527ec/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts.
class PVPlayerSoundCloud implements IPVPlayer {
	private player?: SC.SoundCloudWidget;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLIFrameElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerSoundCloud.ctor');
	}

	private attach = (): Promise<void> => {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				VdbPlayerConsole.debug('SoundCloud player is already attached');

				resolve();
				return;
			}

			this.player = SC.Widget(this.playerElementRef.current);
			this.player.bind(SC.Widget.Events.READY, () => {
				VdbPlayerConsole.debug('SoundCloud player attached');

				resolve();
			});
			this.player.bind(SC.Widget.Events.ERROR, (e) =>
				this.options.onError?.(e),
			);
			this.player.bind(SC.Widget.Events.PLAY, () => this.options.onPlay?.());
			this.player.bind(SC.Widget.Events.PAUSE, () => this.options.onPause?.());
			this.player.bind(SC.Widget.Events.FINISH, () => this.options.onEnded?.());
		});
	};

	private assertPlayerAttached = (): void => {
		VdbPlayerConsole.assert(!!this.player, 'SoundCloud player is not attached');
	};

	private getUrlFromId = (pvId: string): string => {
		var parts = pvId.split(' ');
		var url = `https://api.soundcloud.com/tracks/${parts[0]}`;
		return url;
	};

	public load = async (pv: PVContract): Promise<void> => {
		VdbPlayerConsole.debug(
			'PVPlayerSoundCloud.load',
			JSON.parse(JSON.stringify(pv)),
		);

		VdbPlayerConsole.debug('Attaching SoundCloud player...');

		await this.attach();

		this.assertPlayerAttached();
		if (!this.player) return;

		VdbPlayerConsole.debug('Loading SoundCloud video...');

		this.player.load(this.getUrlFromId(pv.pvId), { auto_play: true });
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerSoundCloud.play');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.play();
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerSoundCloud.pause');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.pause();
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerSoundCloud.seekTo');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.seekTo(seconds * 1000);
	};
}

interface EmbedSoundCloudProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedSoundCloud = React.memo(
	({ playerRef, ...options }: EmbedSoundCloudProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedSoundCloud');

		const playerElementRef = React.useRef<HTMLIFrameElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerSoundCloud(playerElementRef, options);
		}, [playerRef, options]);

		return (
			// eslint-disable-next-line jsx-a11y/iframe-has-title
			<iframe
				ref={playerElementRef}
				src="https://w.soundcloud.com/player/?url="
				frameBorder={0}
				allow="autoplay"
				style={{ width: '100%', height: '100%' }}
			/>
		);
	},
);

export default EmbedSoundCloud;
