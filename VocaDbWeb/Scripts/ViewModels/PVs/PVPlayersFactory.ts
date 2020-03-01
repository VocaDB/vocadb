import { IPVPlayer } from './PVPlayerViewModel';
import PVPlayerFile from './PVPlayerFile';
import PVPlayerNico from './PVPlayerNico';
import PVPlayerSoundCloud from './PVPlayerSoundCloud';
import PVPlayerYoutube from './PVPlayerYoutube';
import PVService from '../../Models/PVs/PVService';

//module vdb.viewModels.pvs {
	
	export default class PVPlayersFactory {
		
		constructor(
			private wrapperElement: HTMLElement,
			public playerElementId: string = 'pv-player') { }

		public createPlayers = (
			songFinishedCallback: () => void = null) => {

			var players: { [index: string]: IPVPlayer; } = {
				File: <IPVPlayer>new PVPlayerFile(this.playerElementId, this.wrapperElement, songFinishedCallback),
				LocalFile: <IPVPlayer>new PVPlayerFile(this.playerElementId, this.wrapperElement, songFinishedCallback, PVService.LocalFile),
				NicoNicoDouga: <IPVPlayer>new PVPlayerNico(this.playerElementId, this.wrapperElement, songFinishedCallback),
				Youtube: <IPVPlayer>new PVPlayerYoutube(this.playerElementId, this.wrapperElement, songFinishedCallback),
				SoundCloud: <IPVPlayer>new PVPlayerSoundCloud(this.playerElementId, this.wrapperElement, songFinishedCallback)
			};

			return players;

		}

	}

//}