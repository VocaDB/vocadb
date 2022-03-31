import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import IPVPlayer from './IPVPlayer';

class PVPlayerYouTube implements IPVPlayer {
	public constructor(
		private readonly playerElementRef: React.MutableRefObject<HTMLDivElement>,
	) {
		console.debug('[VdbPlayer] PVPlayerYouTube.ctor', playerElementRef.current);
	}

	public load = async (pv: PVContract): Promise<void> => {
		console.debug(
			'[VdbPlayer] PVPlayerYouTube.load',
			JSON.parse(JSON.stringify(pv)),
		);
	};

	public play = (): void => {
		console.debug('[VdbPlayer] PVPlayerYouTube.play');
	};

	public pause = (): void => {
		console.debug('[VdbPlayer] PVPlayerYouTube.pause');
	};
}

interface EmbedYouTubeProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const EmbedYouTube = React.memo(
	({ playerRef }: EmbedYouTubeProps): React.ReactElement => {
		console.debug('[VdbPlayer] EmbedYouTube');

		const playerElementRef = React.useRef<HTMLDivElement>(undefined!);

		React.useEffect(() => {
			playerRef.current = new PVPlayerYouTube(playerElementRef);
		}, [playerRef]);

		return (
			<div css={{ width: '100%', height: '100%' }}>
				<div ref={playerElementRef} />
			</div>
		);
	},
);

export default EmbedYouTube;
