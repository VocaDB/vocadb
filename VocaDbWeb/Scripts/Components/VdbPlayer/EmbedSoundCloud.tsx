import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer from './IPVPlayer';

class PVPlayerSoundCloud implements IPVPlayer {
	public constructor() {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.ctor');
	}

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerSoundCloud.load',
			JSON.parse(JSON.stringify(pv)),
		);
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.play');
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerSoundCloud.pause');
	};
}

interface EmbedSoundCloudProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const EmbedSoundCloud = React.memo(
	({ playerRef }: EmbedSoundCloudProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedSoundCloud');

		React.useEffect(() => {
			playerRef.current = new PVPlayerSoundCloud();
		}, [playerRef]);

		return <></>;
	},
);

export default EmbedSoundCloud;
