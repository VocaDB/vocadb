import { PVService } from '@/Models/PVs/PVService';
import { IPVPlayer } from '@/Stores/PVs/PVPlayerStore';
import $ from 'jquery';

export class PVPlayerSoundCloud implements IPVPlayer {
	private player?: SC.SoundCloudWidget;
	readonly service = PVService.SoundCloud;

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
					$(
						`<div id='${this.playerElementId}' src='${window.location.protocol}//w.soundcloud.com/player/' />`,
					),
				);
			}

			this.player = SC.Widget(this.playerElementId);
			this.player.bind(SC.Widget.Events.FINISH, () => {
				if (this.player) this.songFinishedCallback?.();
			});

			this.player.bind(SC.Widget.Events.READY, () => resolve());

			this.player.bind(SC.Widget.Events.ERROR, () => reject());
		});
	};

	detach = (): void => {
		if (this.player) {
			this.player.unbind(SC.Widget.Events.FINISH);
		}

		this.player = undefined;
	};

	private getUrlFromId = (pvId: string): string => {
		const parts = pvId.split(' ');
		const url = `http://api.soundcloud.com/tracks/${parts[0]}`;
		return url;
	};

	play = (pvId?: string): void => {
		if (!this.player) this.attach(false);

		if (pvId) {
			this.player!.load(this.getUrlFromId(pvId), { auto_play: true });
		} else {
			this.player!.play();
		}
	};
}
