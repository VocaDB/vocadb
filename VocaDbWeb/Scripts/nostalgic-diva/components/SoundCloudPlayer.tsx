import React from 'react';

import { LogLevel } from '../players/ILogger';
import { SoundCloudPlayerApi } from '../players/SoundCloudPlayerApi';
import { ensureScriptLoaded } from '../players/ensureScriptLoaded';
import { PlayerContainer, PlayerProps } from './PlayerContainer';

export const SoundCloudPlayer = React.memo(
	({ ...props }: PlayerProps): React.ReactElement => {
		const { logger } = props;

		logger.log(LogLevel.Debug, 'SoundCloudPlayer');

		const loadScript = React.useCallback(async () => {
			await ensureScriptLoaded(
				'https://w.soundcloud.com/player/api.js',
				logger,
			);
		}, [logger]);

		return (
			<PlayerContainer
				{...props}
				loadScript={loadScript}
				playerApiFactory={SoundCloudPlayerApi}
			>
				{(playerElementRef, videoId): React.ReactElement => (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						ref={playerElementRef}
						src={`https://w.soundcloud.com/player/?url=${videoId}`}
						frameBorder={0}
						allow="autoplay"
						style={{ width: '100%', height: '100%' }}
					/>
				)}
			</PlayerContainer>
		);
	},
);
