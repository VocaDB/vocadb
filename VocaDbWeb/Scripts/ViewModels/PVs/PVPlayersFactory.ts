import PVService from '@Models/PVs/PVService';

import PVPlayerFile from './PVPlayerFile';
import PVPlayerNico from './PVPlayerNico';
import PVPlayerSoundCloud from './PVPlayerSoundCloud';
import { IPVPlayer } from './PVPlayerViewModel';
import PVPlayerYoutube from './PVPlayerYoutube';

export default class PVPlayersFactory {
	public constructor(
		private wrapperElement: HTMLElement,
		public playerElementId: string = 'pv-player',
	) {}

	public createPlayers = (
		songFinishedCallback: () => void = null!,
	): { [index: string]: IPVPlayer } => {
		var players: { [index: string]: IPVPlayer } = {
			File: new PVPlayerFile(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			) as IPVPlayer,
			LocalFile: new PVPlayerFile(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
				PVService.LocalFile,
			) as IPVPlayer,
			NicoNicoDouga: new PVPlayerNico(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			) as IPVPlayer,
			Youtube: new PVPlayerYoutube(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			) as IPVPlayer,
			SoundCloud: new PVPlayerSoundCloud(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			) as IPVPlayer,
		};

		return players;
	};
}
