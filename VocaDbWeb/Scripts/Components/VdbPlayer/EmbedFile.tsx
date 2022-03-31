import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer from './IPVPlayer';

class PVPlayerFile implements IPVPlayer {
	public constructor() {
		console.debug('[VdbPlayer] PVPlayerFile.ctor');
	}

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerFile.load',
			JSON.parse(JSON.stringify(pv)),
		);
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerFile.play');
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerFile.pause');
	};
}

interface EmbedFileProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const EmbedFile = React.memo(
	({ playerRef }: EmbedFileProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedFile');

		React.useEffect(() => {
			playerRef.current = new PVPlayerFile();
		}, [playerRef]);

		return <></>;
	},
);

export default EmbedFile;
