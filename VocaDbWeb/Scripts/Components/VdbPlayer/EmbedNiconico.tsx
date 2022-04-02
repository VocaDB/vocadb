import PVContract from '@DataContracts/PVs/PVContract';
import $ from 'jquery';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';
import { getScript } from './getScript';

declare namespace nico {
	export interface NicoPlayer {
		iframeElement: HTMLIFrameElement;
		playerId: string;
		play(): void;
		pause(): void;
	}

	export interface NicoPlayerFactory {
		create(element: HTMLElement, watchId: string): Promise<NicoPlayer>;
	}

	export const enum PlayerStatus {
		Play = 2,
		Pause = 3,
		End = 4,
	}

	export interface PlayerStatusEvent {
		eventName: 'playerStatusChange';
		data: {
			playerStatus: PlayerStatus;
		};
	}

	export interface StatusEvent {
		eventName: 'statusChange';
		data: {
			playerStatus: PlayerStatus;
		};
	}

	export interface MetadataEvent {
		eventName: 'playerMetadataChange';
		data: {
			currentTime: number;
			duration: number;
		};
	}

	export interface LoadCompleteEvent {
		eventName: 'loadComplete';
		data: {
			videoInfo: {
				watchId: string;
			};
		};
	}

	export interface ErrorEvent {
		eventName: 'error';
		data: {
			message: string;
		};
	}

	export interface PlayerErrorEvent {
		eventName: 'player-error:video:play' | 'player-error:video:seek';
		data: {
			message: string;
		};
	}

	export interface UnknownEvent {
		eventName: string;
		data: any;
	}

	type EventData =
		| PlayerStatusEvent
		| StatusEvent
		| MetadataEvent
		| LoadCompleteEvent
		| ErrorEvent
		| PlayerErrorEvent
		| UnknownEvent;

	export interface PlayerEvent {
		origin: string;
		data: EventData;
	}
}

declare global {
	interface Window {
		onNicoPlayerFactoryReady: (callback: nico.NicoPlayerFactory) => void;
	}
}

// Code from: https://github.com/VocaDB/vocadb/blob/a4b5f9d8186772d7e6f58f997bbcbb51509d2539/VocaDbWeb/Scripts/ViewModels/PVs/PVPlayerNico.ts.
class PVPlayerNiconico implements IPVPlayer {
	private static origin = 'https://embed.nicovideo.jp';

	private player?: nico.NicoPlayer;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLDivElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerNiconico.ctor');
	}

	private static scriptLoaded = false;
	private static playerFactory?: nico.NicoPlayerFactory;

	private loadScript = (): Promise<void> => {
		return new Promise(async (resolve, reject) => {
			if (PVPlayerNiconico.scriptLoaded) {
				VdbPlayerConsole.debug('Niconico script is already loaded');

				resolve();
				return;
			}

			window.onNicoPlayerFactoryReady = (factory): void => {
				PVPlayerNiconico.playerFactory = factory;

				VdbPlayerConsole.debug('Niconico player factory ready', factory);

				window.addEventListener('message', (e: nico.PlayerEvent) => {
					if (e.origin !== PVPlayerNiconico.origin) return;

					switch (e.data.eventName) {
						case 'playerStatusChange':
							VdbPlayerConsole.debug(
								`Niconico player status changed: ${e.data.data.playerStatus}`,
							);

							switch (e.data.data.playerStatus) {
								case nico.PlayerStatus.Play:
									this.options.onPlay?.();
									break;

								case nico.PlayerStatus.Pause:
									this.options.onPause?.();
									break;

								case nico.PlayerStatus.End:
									this.options.onEnded?.();
									break;
							}
							break;

						case 'statusChange':
							VdbPlayerConsole.debug(
								`Niconico status changed: ${e.data.data.playerStatus}`,
							);
							break;

						case 'playerMetadataChange':
							break;

						case 'loadComplete':
							VdbPlayerConsole.debug('Niconico load completed');

							// TODO: Implement.
							break;

						case 'error':
							// TODO: Implement.

							this.options.onError?.(e.data);
							break;

						case 'player-error:video:play':
						case 'player-error:video:seek':
							this.options.onError?.(e.data);
							break;

						default:
							VdbPlayerConsole.warn(
								'Niconico message',
								e.data.eventName,
								e.data.data,
							);
							break;
					}
				});

				resolve();
			};

			try {
				VdbPlayerConsole.debug('Loading Niconico script...');

				await getScript('https://static.vocadb.net/script/nico/api.js');

				PVPlayerNiconico.scriptLoaded = true;

				VdbPlayerConsole.debug('Niconico script loaded');
			} catch {
				VdbPlayerConsole.error('Failed to load Niconico script');

				reject();
			}
		});
	};

	public load = async (pv: PVContract): Promise<void> => {
		VdbPlayerConsole.debug(
			'PVPlayerNiconico.load',
			JSON.parse(JSON.stringify(pv)),
		);

		await this.loadScript();

		$(this.playerElementRef.current).empty();

		VdbPlayerConsole.assert(
			!!PVPlayerNiconico.playerFactory,
			'Niconico playerFactory is not defined',
		);
		if (!PVPlayerNiconico.playerFactory) return;

		this.player = await PVPlayerNiconico.playerFactory.create(
			this.playerElementRef.current,
			pv.pvId,
		);

		VdbPlayerConsole.assert(
			!!this.player.iframeElement,
			'player.iframeElement is not defined',
		);

		this.player.iframeElement.width = '100%';
		this.player.iframeElement.height = '100%';

		// As of Chrome 66, videos must be muted in order to play automatically.
		// See also https://github.com/cookpete/react-player/blob/137f1fc3569fc56e14640df7c5c9af1ceba7f992/README.md#autoplay and https://kiite.jp/faq#browser-setting.
		this.mute();
	};

	private assertPlayerAttached = (): void => {
		VdbPlayerConsole.assert(!!this.player, 'Niconico player is not attached');
	};

	// Code from: https://blog.hayu.io/web/create/nicovideo-embed-player-api/.
	private postMessage = (message: any): void => {
		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.iframeElement.contentWindow?.postMessage(
			{
				...message,
				playerId: this.player.playerId,
				sourceConnectorType: 1,
			},
			PVPlayerNiconico.origin,
		);
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.play');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.play();
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.pause');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.pause();
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.seekTo');

		this.assertPlayerAttached();
		if (!this.player) return;

		this.postMessage({ eventName: 'seek', data: { time: seconds * 1000 } });
	};

	public mute = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.mute');

		this.postMessage({
			eventName: 'mute',
			data: { mute: true },
		});
	};

	public unmute = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.unmute');

		this.postMessage({
			eventName: 'mute',
			data: { mute: false },
		});
	};
}

interface EmbedNiconicoProps extends IPVPlayerOptions {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedNiconico = React.memo(
	({ playerRef, ...options }: EmbedNiconicoProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedNiconico');

		const playerElementRef = React.useRef<HTMLDivElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerNiconico(playerElementRef, options);
		}, [playerRef, options]);

		return (
			<div ref={playerElementRef} style={{ width: '100%', height: '100%' }} />
		);
	},
);

export default EmbedNiconico;
