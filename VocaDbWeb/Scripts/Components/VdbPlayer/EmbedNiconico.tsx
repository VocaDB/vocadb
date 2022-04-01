import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';

class PVPlayerNiconico implements IPVPlayer {
	public constructor() {
		VdbPlayerConsole.debug('PVPlayerNiconico.ctor');
	}

	public load = async (pv: PVContract): Promise<void> => {
		VdbPlayerConsole.debug(
			'PVPlayerNiconico.load',
			JSON.parse(JSON.stringify(pv)),
		);
	};

	public play = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.play');
	};

	public pause = (): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.pause');
	};

	public seekTo = (seconds: number): void => {
		VdbPlayerConsole.debug('PVPlayerNiconico.seekTo');
	};
}

interface EmbedNiconicoProps {
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedNiconico = React.memo(
	({ playerRef }: EmbedNiconicoProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedNiconico');

		React.useEffect(() => {
			playerRef.current = new PVPlayerNiconico();
		}, [playerRef]);

		return <></>;
	},
);

export default EmbedNiconico;
