import PVService from '@Models/PVs/PVService';
import $ from 'jquery';

import { IPVPlayer } from './PVPlayerStore';

export default class PVPlayerFile implements IPVPlayer {
	public readonly service;
	private player?: HTMLAudioElement;

	public constructor(
		private readonly playerElementId: string,
		private readonly wrapperElement: string,
		public readonly songFinishedCallback?: () => void,
		service: PVService = PVService.File,
	) {
		this.service = service;
	}

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
				$(`<audio id='${this.playerElementId}' />`),
			);
		}

		this.player = $(`#${this.playerElementId}`)[0] as HTMLAudioElement;
		this.player.onended = (): void => {
			if (this.player) this.songFinishedCallback?.();
		};

		readyCallback?.();
	};

	public detach = (): void => {
		if (this.player) {
			this.player.onended = null;
		}

		this.player = undefined;
	};

	public play = (pvId?: string): void => {
		if (!this.player) this.attach(false);

		if (pvId) {
			this.player!.src = pvId;
			this.player!.autoplay = true;
		} else {
			this.player!.play();
		}
	};
}
