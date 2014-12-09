
module vdb.viewModels.pvs {

	import cls = vdb.models;

	export class PVPlayerFile implements IPVPlayer {

		constructor(
			private playerElementId: string,
			private wrapperElement: HTMLElement,
			public songFinishedCallback: () => void = null) {

		}

		public attach = (reset: boolean = false, readyCallback?: () => void) => {

			if (!reset && this.player) {
				if (readyCallback)
					readyCallback();
				return;
			}

			if (reset) {
				$(this.wrapperElement).empty();
				$(this.wrapperElement).append($("<audio id='" + this.playerElementId + "' />"));
			}

			this.player = <HTMLAudioElement>$("#" + this.playerElementId)[0];
			this.player.onended = () => {
				if (this.player && this.songFinishedCallback)
					this.songFinishedCallback();				
			}

			if (readyCallback)
				readyCallback();				

		}

		public detach = () => {

			if (this.player) {
				this.player.onended = null;
			}

			this.player = null;

		}

		private player: HTMLAudioElement = null;

		public play = (pvId?: string) => {

			if (!this.player)
				this.attach(false);

			if (pvId) {
				this.player.src = pvId;
				this.player.autoplay = true;
			} else {
				this.player.play();

			}

		}

		public service = cls.pvs.PVService.File;

	}

}