import { PVService } from '@/Models/PVs/PVService';
import { IPVPlayer } from '@/Stores/PVs/PVPlayerStore';
import $ from 'jquery';

export class PVPlayerYoutube implements IPVPlayer {
	private player?: YT.Player;
	readonly service = PVService.Youtube;

	constructor(
		private readonly playerElementId: string,
		private readonly wrapperElement: string,
		readonly songFinishedCallback?: () => void,
	) {}

	attach = (reset: boolean = false): Promise<void> => {
		return new Promise((resolve, reject) => {
			if (!reset && this.player) {
				resolve();
				return;
			}

			if (reset) {
				$(this.wrapperElement).empty();
				$(this.wrapperElement).append(
					$(`<div id='${this.playerElementId}' />`),
				);
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
					onReady: (): void => resolve(),
					onError: (): void => reject(),
				},
			});
		});
	};

	detach = (): void => {
		this.player = undefined;
	};

	private doPlay = (pvId?: string): void => {
		if (pvId) {
			this.player!.loadVideoById(pvId);
		} else {
			this.player!.playVideo();
		}
	};

	play = (pvId?: string): void => {
		if (!this.player) {
			this.attach(false).then(() => this.doPlay(pvId));
		} else {
			this.doPlay(pvId);
		}
	};
}
