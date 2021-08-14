import { IPVPlayer } from './PVPlayerStore';

export default class PVPlayersFactory {
	public constructor(public readonly playerElementId: string = 'pv-player') {}

	public createPlayers = (
		songFinishedCallback?: () => void,
	): { [index: string]: IPVPlayer } => {
		return {} /* TODO */;
	};
}
