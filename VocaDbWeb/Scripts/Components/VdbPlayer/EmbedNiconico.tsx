import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer from './IPVPlayer';

class PVPlayerNiconico implements IPVPlayer {
	public constructor() {
		console.debug('[VdbPlayer] PVPlayerNiconico.ctor');
	}

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerNiconico.load',
			JSON.parse(JSON.stringify(pv)),
		);
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerNiconico.play');
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerNiconico.pause');
	};

	public seekTo = (seconds: number): void => {
		console.debug('[VdbPlayer] PVPlayerNiconico.seekTo');
	};
}

interface EmbedNiconicoProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const EmbedNiconico = React.memo(
	({ playerRef }: EmbedNiconicoProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedNiconico');

		React.useEffect(() => {
			playerRef.current = new PVPlayerNiconico();
		}, [playerRef]);

		return <></>;
	},
);

export default EmbedNiconico;
