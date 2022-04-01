import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer, { IPVPlayerOptions } from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';
import { getScript } from './getScript';

declare namespace nico {
	export interface NicoPlayer {
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

	export interface StatusEvent {
		eventName: 'playerStatusChange';
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

	type EventData =
		| StatusEvent
		| MetadataEvent
		| LoadCompleteEvent
		| ErrorEvent
		| PlayerErrorEvent;

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
					if (e.origin !== 'https://embed.nicovideo.jp') return;

					VdbPlayerConsole.debug(
						'Niconico message',
						e.data.eventName,
						e.data.data,
					);

					switch (e.data.eventName) {
						case 'playerStatusChange':
							VdbPlayerConsole.debug(
								`Niconico status changed: ${e.data.data.playerStatus}`,
							);

							switch (e.data.data.playerStatus) {
								case nico.PlayerStatus.End:
									this.options.onEnded?.();
									break;
							}
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
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.play');
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.pause');
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.seekTo');
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

		return <div ref={playerElementRef} />;
	},
);

export default EmbedNiconico;
