import React from 'react';

import { LogLevel } from '../players/ILogger';
import { YouTubePlayerApi } from '../players/YouTubePlayerApi';
import { ensureScriptLoaded } from '../players/ensureScriptLoaded';
import { PlayerContainer, PlayerProps } from './PlayerContainer';

export const YouTubePlayer = React.memo(
	({ ...props }: PlayerProps): React.ReactElement => {
		const { logger } = props;

		logger.log(LogLevel.Debug, 'YouTubePlayer');

		const loadScript = React.useCallback((): Promise<void> => {
			return new Promise(async (resolve, reject) => {
				const first = await ensureScriptLoaded(
					'https://www.youtube.com/iframe_api',
					logger,
				);

				if (first) {
					// https://stackoverflow.com/a/18154942.
					window.onYouTubeIframeAPIReady = (): void => {
						logger.log(LogLevel.Debug, 'YouTube iframe API ready');
						resolve();
					};
				} else {
					resolve();
				}
			});
		}, [logger]);

		return (
			<PlayerContainer
				{...props}
				loadScript={loadScript}
				playerApiFactory={YouTubePlayerApi}
			>
				{(playerElementRef): React.ReactElement => (
					<div style={{ width: '100%', height: '100%' }}>
						<div ref={playerElementRef} />
					</div>
				)}
			</PlayerContainer>
		);
	},
);
