
module vdb.viewModels.pvs {
	
	export class PVPlayersFactory {
		
		constructor(
			private wrapperElement: HTMLElement,
			public playerElementId: string = 'pv-player') { }

		public createPlayers = (
			songFinishedCallback: () => void = null) => {

			var players: { [index: string]: IPVPlayer; } = {
				File: <IPVPlayer>new PVPlayerFile(this.playerElementId, this.wrapperElement, songFinishedCallback),
				Youtube: <IPVPlayer>new PVPlayerYoutube(this.playerElementId, this.wrapperElement, songFinishedCallback),
				SoundCloud: <IPVPlayer>new PVPlayerSoundCloud(this.playerElementId, this.wrapperElement, songFinishedCallback)
			};

			return players;

		}

	}

}