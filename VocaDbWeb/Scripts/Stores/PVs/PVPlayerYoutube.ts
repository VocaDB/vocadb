import PVService from '@Models/PVs/PVService';
import $ from 'jquery';

import { IPVPlayer } from './PVPlayerStore';

export default class PVPlayerYoutube implements IPVPlayer {
	private player?: YT.Player;
	public readonly service = PVService.Youtube;

	public constructor(
		private readonly playerElementId: string,
		private readonly wrapperElement: string,
		public readonly songFinishedCallback?: () => void,
	) {}

	public attach = (
		reset: boolean = false,
		readyCallback?: () => void,
	): void => {
		if (!reset && this.player) {
			readyCallback?.();
			return;
		}

		if (reset) {
			$(this.wrapperElement).empty();
			$(this.wrapperElement).append($(`<div id='${this.playerElementId}' />`));
		}

		this.player = new YT.Player(this.playerElementId, {
			width: 560,
			height: 315,
			events: {
				onStateChange: (event: YT.EventArgs): void => {
					// This will still be fired once if the user disabled autoplay mode.
					if (this.player && event.data === YT.PlayerState.ENDED) {
						this.songFinishedCallback?.();
					}
				},
				onReady: (): void => {
					readyCallback?.();
				},
				onError: (): void => {
					// Some delay, to let the user read the error message and to prevent infinite loop
					setTimeout(() => {
						if (this.player) {
							this.songFinishedCallback?.();
						}
					}, 3000);
				},
			},
		});
	};

	public detach = (): void => {
		this.player = undefined;
	};

	private doPlay = (pvId?: string): void => {
		if (pvId) {
			this.player!.loadVideoById(pvId);
		} else {
			this.player!.playVideo();
		}
	};

	public play = (pvId?: string): void => {
		if (!this.player) {
			this.attach(false, () => this.doPlay(pvId));
		} else {
			this.doPlay(pvId);
		}
	};
}
