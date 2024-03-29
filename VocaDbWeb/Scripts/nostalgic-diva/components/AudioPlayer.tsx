import { functions } from '@/Shared/GlobalFunctions';
import { useVdb } from '@/VdbContext';
import React from 'react';

import { AudioPlayerApi } from '../players/AudioPlayerApi';
import { LogLevel } from '../players/ILogger';
import { PlayerContainer, PlayerProps } from './PlayerContainer';

export const AudioPlayer = React.memo(
	({ ...props }: PlayerProps): React.ReactElement => {
		const { logger } = props;
		const { values } = useVdb();

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
						src={
							videoId.startsWith('http')
								? videoId
								: functions.mergeUrls(
										values.staticContentHost,
										`/media/${videoId}`,
								  )
						}
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
