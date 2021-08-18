import PVService from '@Models/PVs/PVService';
import $ from 'jquery';

import { IPVPlayer } from './PVPlayerStore';

export default class PVPlayerSoundCloud implements IPVPlayer {
	private player?: SC.SoundCloudWidget;
	public readonly service = PVService.SoundCloud;

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

		this.player.bind(SC.Widget.Events.READY, () => {
			readyCallback?.();
		});

		this.player.bind(SC.Widget.Events.ERROR, () => {
			// Some delay, to let the user read the error message and to prevent infinite loop
			setTimeout(() => {
				if (this.player) this.songFinishedCallback?.();
			}, 3000);
		});
	};

	public detach = (): void => {
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

	public play = (pvId?: string): void => {
		if (!this.player) this.attach(false);

		if (pvId) {
			this.player!.load(this.getUrlFromId(pvId), { auto_play: true });
		} else {
			this.player!.play();
		}
	};
}
