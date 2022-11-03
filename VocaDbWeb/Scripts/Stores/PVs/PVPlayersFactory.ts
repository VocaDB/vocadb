import { PVService } from '@/Models/PVs/PVService';
import { PVPlayerFile } from '@/Stores/PVs/PVPlayerFile';
import { PVPlayerNico } from '@/Stores/PVs/PVPlayerNico';
import { PVPlayerSoundCloud } from '@/Stores/PVs/PVPlayerSoundCloud';
import { IPVPlayer } from '@/Stores/PVs/PVPlayerStore';
import { PVPlayerYoutube } from '@/Stores/PVs/PVPlayerYoutube';

export class PVPlayersFactory {
	constructor(
		private readonly wrapperElement: string = '#pv-player-wrapper',
		readonly playerElementId: string = 'pv-player',
	) {}

	createPlayers = (
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
