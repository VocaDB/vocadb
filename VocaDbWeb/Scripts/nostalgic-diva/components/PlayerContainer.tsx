import React from 'react';

import { ILogger, LogLevel } from '../players/ILogger';
import {
	IPlayerApi,
	PlayerApi,
	PlayerOptions,
	PlayerType,
} from '../players/PlayerApi';
import { PlayerApiImpl } from '../players/PlayerApiImpl';
import usePreviousDistinct from './usePreviousDistinct';

export interface PlayerProps {
	logger: ILogger;
	type: PlayerType;
	playerApiRef: React.MutableRefObject<IPlayerApi | undefined> | undefined;
	videoId: string;
	options: PlayerOptions | undefined;
}

interface PlayerContainerProps<
	TElement extends HTMLElement,
	TPlayer extends PlayerApiImpl<TElement>,
> extends PlayerProps {
	loadScript: (() => Promise<void>) | undefined;
	playerApiFactory: new (
		logger: ILogger,
		playerElementRef: React.MutableRefObject<TElement>,
		options: PlayerOptions | undefined,
	) => TPlayer;
	children: (
		playerElementRef: React.MutableRefObject<TElement>,
		videoId: string,
	) => React.ReactNode;
}

export const PlayerContainer = <
	TElement extends HTMLElement,
	TPlayer extends PlayerApiImpl<TElement>,
>({
	logger,
	type,
	playerApiRef,
	videoId,
	options,
	loadScript,
	playerApiFactory,
	children,
}: PlayerContainerProps<TElement, TPlayer>): React.ReactElement<
	PlayerContainerProps<TElement, TPlayer>
> => {
	logger.log(LogLevel.Debug, 'PlayerContainer');

	const videoIdRef = React.useRef(videoId);

	// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
	const playerElementRef = React.useRef<TElement>(undefined!);

	const [playerApi, setPlayerApi] = React.useState<IPlayerApi>();

	// Make sure that `options` do not change between re-rendering.
	React.useEffect(() => {
		const playerApi = new PlayerApi(
			logger,
			type,
			playerElementRef,
			options,
			loadScript,
			playerApiFactory,
		);

		if (playerApiRef) playerApiRef.current = playerApi;

		playerApi
			.attach(videoIdRef.current)
			.then(() => setPlayerApi(playerApi));

		return (): void => {
			if (playerApiRef) {
				if (playerApi !== playerApiRef.current) {
					throw new Error('playerApi differs');
				}
			}

			playerApi.detach().then(() => setPlayerApi(undefined));
		};
	}, [logger, type, options, loadScript, playerApiFactory, playerApiRef]);

	const previousVideoId = usePreviousDistinct(videoId);
	React.useEffect(() => {
		// If `previousVideoId` is undefined, then it means that the video has already been loaded by either
		// 1. `<audio>`s `src` attribute (e.g. `AudioPlayer`),
		// 2. `<iframe>`'s `src` attribute (e.g. `NiconicoPlayer`, `SoundCloudPlayer` and `VimeoPlayer`), or
		// 3. the `attach` method of the player API (e.g. `YouTubePlayer`).
		if (previousVideoId === undefined) return;

		playerApi?.loadVideo(videoId);
	}, [previousVideoId, videoId, playerApi]);

	// Make sure that `videoId` does not change between re-rendering.
	return <>{children(playerElementRef, videoIdRef.current)}</>;
};
