import PVService from '@Models/PVs/PVService';

import PVPlayerFile from './PVPlayerFile';
import PVPlayerNico from './PVPlayerNico';
import PVPlayerSoundCloud from './PVPlayerSoundCloud';
import { IPVPlayer } from './PVPlayerStore';
import PVPlayerYoutube from './PVPlayerYoutube';

export default class PVPlayersFactory {
	public constructor(
		private readonly wrapperElement: string = '#pv-player-wrapper',
		public readonly playerElementId: string = 'pv-player',
	) {}

	public createPlayers = (
		songFinishedCallback?: () => void,
	): { [index: string]: IPVPlayer } => {
		return {
			File: new PVPlayerFile(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			),
			LocalFile: new PVPlayerFile(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
				PVService.LocalFile,
			),
			NicoNicoDouga: new PVPlayerNico(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			),
			Youtube: new PVPlayerYoutube(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			),
			SoundCloud: new PVPlayerSoundCloud(
				this.playerElementId,
				this.wrapperElement,
				songFinishedCallback,
			),
		};
	};
}
