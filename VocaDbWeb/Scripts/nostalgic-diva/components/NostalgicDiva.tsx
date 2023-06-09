import React from 'react';

import { LogLevel } from '../players/ILogger';
import { PlayerOptions, PlayerType } from '../players/PlayerApi';
import { AudioPlayer } from './AudioPlayer';
import { NiconicoPlayer } from './NiconicoPlayer';
import { useNostalgicDiva } from './NostalgicDivaProvider';
import { PlayerProps } from './PlayerContainer';
import { SoundCloudPlayer } from './SoundCloudPlayer';
import { VimeoPlayer } from './VimeoPlayer';
import { YouTubePlayer } from './YouTubePlayer';

const players: Record<PlayerType, React.ElementType<PlayerProps>> = {
	Audio: AudioPlayer,
	Niconico: NiconicoPlayer,
	SoundCloud: SoundCloudPlayer,
	Vimeo: VimeoPlayer,
	YouTube: YouTubePlayer,
};

interface NostalgicDivaProps {
	type: PlayerType;
	videoId: string;
	options?: PlayerOptions;
}

export const NostalgicDiva = React.memo(
	({ type, videoId, options }: NostalgicDivaProps): React.ReactElement => {
		const { logger, playerApiRef } = useNostalgicDiva();

		logger.log(LogLevel.Debug, 'NostalgicDiva');

		const Player = players[type];
		return (
			<Player
				logger={logger}
				type={type}
				playerApiRef={playerApiRef}
				videoId={videoId}
				options={options}
			/>
		);
	},
);
