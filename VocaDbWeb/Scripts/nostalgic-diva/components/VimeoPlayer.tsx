import React from 'react';

import { LogLevel } from '../players/ILogger';
import { VimeoPlayerApi } from '../players/VimeoPlayerApi';
import { ensureScriptLoaded } from '../players/ensureScriptLoaded';
import { PlayerContainer, PlayerProps } from './PlayerContainer';

export const VimeoPlayer = React.memo(
	({ ...props }: PlayerProps): React.ReactElement => {
		const { logger } = props;

		logger.log(LogLevel.Debug, 'VimeoPlayer');

		const loadScript = React.useCallback(async () => {
			await ensureScriptLoaded(
				'https://player.vimeo.com/api/player.js',
				logger,
			);
		}, [logger]);

		return (
			<PlayerContainer
				{...props}
				loadScript={loadScript}
				playerApiFactory={VimeoPlayerApi}
			>
				{(playerElementRef, videoId): React.ReactElement => (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						ref={playerElementRef}
						src={`https://player.vimeo.com/video/${videoId}`}
						frameBorder={0}
						allow="autoplay"
						style={{ width: '100%', height: '100%' }}
					/>
				)}
			</PlayerContainer>
		);
	},
);
