import PVContract from '@DataContracts/PVs/PVContract';
import qs from 'qs';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';

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

	private player?: HTMLIFrameElement;

	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLIFrameElement>,
		private readonly options: IPVPlayerOptions,
	) {
		VdbPlayerConsole.debug('PVPlayerNiconico.ctor');
	}

	private attach = (): Promise<void> => {
		return new Promise((resolve, reject /* TODO: Reject. */) => {
			if (this.player) {
				VdbPlayerConsole.debug('Niconico player is already attached');

				resolve();
				return;
			}

			this.player = this.playerElementRef.current;

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

			VdbPlayerConsole.debug('Niconico player attached');

			resolve();
		});
	};

	private assertPlayerAttached = (): void => {
		VdbPlayerConsole.assert(!!this.player, 'Niconico player is not attached');
	};

	public load = async (pv: PVContract): Promise<void> => {
		return new Promise(async (resolve, reject /* TODO: Reject. */) => {
			VdbPlayerConsole.debug(
				'PVPlayerNiconico.load',
				JSON.parse(JSON.stringify(pv)),
			);

			await this.attach();

			this.assertPlayerAttached();
			if (!this.player) return;

			// Wait for iframe to load.
			this.player.onload = (): void => {
				this.assertPlayerAttached();
				if (!this.player) return;

				this.player.onload = null;

				VdbPlayerConsole.debug('Niconico iframe loaded');

				resolve();
			};

			this.player.src = `https://embed.nicovideo.jp/watch/${
				pv.pvId
			}?${qs.stringify({
				jsapi: 1,
				playerId: 1,
			})}`;
		});
	};

	// Code from: https://blog.hayu.io/web/create/nicovideo-embed-player-api/.
	private postMessage = (message: any): void => {
		this.assertPlayerAttached();
		if (!this.player) return;

		this.player.contentWindow?.postMessage(
			{
				...message,
				playerId: '1' /* Needs to be a string, not a number. */,
				sourceConnectorType: 1,
			},
			PVPlayerNiconico.origin,
		);
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.play');

		this.postMessage({ eventName: 'play' });
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.pause');

		this.postMessage({ eventName: 'pause' });
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.seekTo');

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

		const playerElementRef = React.useRef<HTMLIFrameElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerNiconico(playerElementRef, options);
		}, [playerRef, options]);

		return (
			<div style={{ width: '100%', height: '100%' }}>
				{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
				<iframe
					ref={playerElementRef}
					width="100%"
					height="100%"
					allowFullScreen
					style={{ border: 'none' }}
					// The player has to have the allow="autoplay" attribute.
					// Otherwise it throws a NotAllowedError: "play() failed because the user didn't interact with the document first".
					// See also: https://github.com/vimeo/player.js/issues/389.
					// NOTE: An iframe element created by `PVPlayerNiconico.playerFactory.create` doesn't have the allow="autoplay" attribute,
					// which causes the above issue when trying to autoplay a video.
					allow="autoplay; fullscreen"
				/>
			</div>
		);
	},
);

export default EmbedNiconico;
