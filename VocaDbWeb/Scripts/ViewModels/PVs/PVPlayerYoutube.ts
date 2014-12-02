
module vdb.viewModels.pvs {

	import cls = vdb.models;

	export class PVPlayerYoutube implements IPVPlayer {

		constructor(
			private playerElementId: string,
			private wrapperElement: HTMLElement,
			public songFinishedCallback: () => void = null) {

		}

		public attach = (reset: boolean = false, readyCallback?: () => void) => {

			if (!reset && this.player)
				return;

			if (reset) {
				$(this.wrapperElement).empty();
				$(this.wrapperElement).append($("<div id='" + this.playerElementId + "' />"));
			}

			this.player = new YT.Player(this.playerElementId, {
				events: {
					'onStateChange': (event: YT.EventArgs) => {

						// This will still be fired once if the user disabled autoplay mode.
						if (this.player && event.data == YT.PlayerState.ENDED && this.songFinishedCallback) {
							this.songFinishedCallback();
						}

					},
					'onReady': () => {
						if (readyCallback)
							readyCallback();
					}
				}
			});

		}

		public detach = () => {
			this.player = null;
		}

		private player: YT.Player = null;

		public play = (pvId) => {

			if (!this.player)
				this.attach(false);

			if (pvId) {
				this.player.loadVideoById(pvId);				
			} else {
				this.player.playVideo();
			}

		}

		public service = cls.pvs.PVService.Youtube;

	}

}