import React from 'react';

import { AudioPlayerApi } from '../players/AudioPlayerApi';
import { LogLevel } from '../players/ILogger';
import { PlayerContainer, PlayerProps } from './PlayerContainer';

export const AudioPlayer = React.memo(
	({ ...props }: PlayerProps): React.ReactElement => {
		const { logger } = props;

		logger.log(LogLevel.Debug, 'AudioPlayer');

		return (
			<PlayerContainer
				{...props}
				loadScript={undefined}
				playerApiFactory={AudioPlayerApi}
			>
				{(playerElementRef, videoId): React.ReactElement => (
					<audio
						ref={playerElementRef}
						src={videoId}
						style={{ width: '100%', height: '100%' }}
						preload="auto"
						autoPlay
						controls
					/>
				)}
			</PlayerContainer>
		);
	},
);
