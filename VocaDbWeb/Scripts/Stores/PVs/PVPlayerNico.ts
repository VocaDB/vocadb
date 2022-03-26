import PVService from '@Models/PVs/PVService';
import $ from 'jquery';

import { IPVPlayer } from './PVPlayerStore';

declare namespace nico {
	export interface NicoPlayerFactory {
		create(element: HTMLElement, watchId: string): Promise<NicoPlayer>;
	}

	export interface PlayerEvent {
		data: EventData;
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

	type EventData = StatusEvent | MetadataEvent | LoadCompleteEvent | ErrorEvent;

	export interface NicoPlayer {
		play(): void;
		pause(): void;
	}

	export const enum PlayerStatus {
		Play = 2,
		Pause = 3,
		End = 4,
	}
}

declare global {
	interface Window {
		onNicoPlayerFactoryReady: (callback: nico.NicoPlayerFactory) => void;
	}
}

/*
		Note: I'm not terrible happy about the implementation for now.
		Can't seem to find a way to attach to already loaded player, so we're always loading a new player.
		The current PV is saved into variable "loadedPv" when the original player is loaded.
	*/
export default class PVPlayerNico implements IPVPlayer {
	private static scriptLoaded: boolean = false;
	private static playerFactory?: nico.NicoPlayerFactory;
	private currentPv?: string;
	private loadedPv?: string;
	private player?: nico.NicoPlayer;
	public readonly service = PVService.NicoNicoDouga;

	public constructor(
		private readonly playerElementId: string,
		private readonly wrapperElement: string,
		public readonly songFinishedCallback?: () => void,
	) {
		window.addEventListener('message', (e: nico.PlayerEvent) => {
			if (e.data.eventName === 'loadComplete') {
				// Save the original loaded PV. This will be used to reconstruct the JavaScript object in play function.
				this.loadedPv = e.data.data.videoInfo.watchId;
			}
		});
	}

	public attach = (
		reset: boolean = false,
		readyCallback?: () => void,
	): void => {
		if (reset) {
			$(this.wrapperElement).empty();
			$(this.wrapperElement).append($(`<div id='${this.playerElementId}' />`));
		}

		window.onNicoPlayerFactoryReady = (factory): void => {
			PVPlayerNico.playerFactory = factory;
			readyCallback?.();
		};

		if (!PVPlayerNico.scriptLoaded) {
			$.getScript('https://static.vocadb.net/script/nico/api.js').then(() => {
				PVPlayerNico.scriptLoaded = true;

				window.addEventListener('message', (e: nico.PlayerEvent) => {
					if (e.data.eventName === 'playerStatusChange') {
						if (e.data.data.playerStatus === nico.PlayerStatus.End) {
							this.songFinishedCallback?.();
						}
					}
					if (e.data.eventName === 'loadComplete') {
						this.loadedPv = e.data.data.videoInfo.watchId;
					}
					if (e.data.eventName === 'error') {
						const currentPv = this.loadedPv;
						window.setTimeout(() => {
							if (currentPv === this.loadedPv) this.songFinishedCallback?.();
						}, 3900);
					}
				});

				readyCallback?.();
			});
		} else {
			readyCallback?.();
		}
	};

	public detach = (): void => {
		this.player = undefined;
	};

	public play = (pvId?: string): void => {
		if (!this.player || this.currentPv !== pvId) {
			if (!pvId) {
				pvId = this.loadedPv;
			}

			if (!pvId) return;

			$(`#${this.playerElementId}`).empty();
			PVPlayerNico.playerFactory!.create(
				$(`#${this.playerElementId}`)[0],
				pvId,
			).then((player) => {
				this.player = player;
				this.player.play();
			});
		} else {
			this.player.play();
		}
	};
}
