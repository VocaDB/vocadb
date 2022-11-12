import { PVService } from '@/Models/PVs/PVService';
import { IPVPlayer } from '@/Stores/PVs/PVPlayerStore';
import $ from 'jquery';

export class PVPlayerFile implements IPVPlayer {
	readonly service;
	private player?: HTMLAudioElement;

	constructor(
		private readonly playerElementId: string,
		private readonly wrapperElement: string,
		readonly songFinishedCallback?: () => void,
		service: PVService = PVService.File,
	) {
		this.service = service;
	}

	attach = (reset: boolean = false): Promise<void> => {
		return new Promise((resolve, reject) => {
			if (!reset && this.player) {
				resolve();
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

			resolve();
		});
	};

	detach = (): void => {
		if (this.player) {
			this.player.onended = null;
		}

		this.player = undefined;
	};

	play = (pvId?: string): void => {
		if (!this.player) this.attach(false);

		if (pvId) {
			this.player!.src = pvId;
			this.player!.autoplay = true;
		} else {
			this.player!.play();
		}
	};
}
