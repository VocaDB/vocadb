import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';

// Code from: https://github.com/VocaDB/vocadb/blob/e147650a8f1f85c8fa865d0ab562126c278527ec/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts.
class PVPlayerSoundCloud implements IPVPlayer {
	private player?: SC.SoundCloudWidget;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLIFrameElement>,
		private readonly options: IPVPlayerOptions,
	) {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.ctor');
	}

	private attach = (): Promise<void> => {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				console.debug('[VdbPlayer] SoundCloud player is already attached');

				resolve();
				return;
			}

			this.player = SC.Widget(this.playerElementRef.current);
			this.player.bind(SC.Widget.Events.READY, () => {
				console.debug('[VdbPlayer] SoundCloud player attached');

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

	private getUrlFromId = (pvId: string): string => {
		var parts = pvId.split(' ');
		var url = `https://api.soundcloud.com/tracks/${parts[0]}`;
		return url;
	};

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerSoundCloud.load',
			JSON.parse(JSON.stringify(pv)),
		);

		console.debug('[VdbPlayer] Attaching SoundCloud player...');

		await this.attach();

		if (!this.player) {
			console.assert('[VdbPlayer] SoundCloud player is not attached');
			return;
		}

		console.debug('[VdbPlayer] Loading SoundCloud video...');

		this.player.load(this.getUrlFromId(pv.pvId), { auto_play: true });
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.play');

		if (!this.player) {
			console.assert('[VdbPlayer] SoundCloud player is not attached');
			return;
		}

		this.player.play();
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.pause');

		if (!this.player) {
			console.assert('[VdbPlayer] SoundCloud player is not attached');
			return;
		}

		this.player.pause();
	};

	public seekTo = (seconds: number): void => {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.seekTo');

		if (!this.player) {
			console.assert('[VdbPlayer] SoundCloud player is not attached');
			return;
		}

		this.player.seekTo(seconds * 1000);
	};
}

interface EmbedSoundCloudProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedSoundCloud = React.memo(
	({ playerRef, ...options }: EmbedSoundCloudProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedSoundCloud');

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
