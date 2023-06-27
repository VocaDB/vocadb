import React from 'react';
import { IPlayerApi, usePlayerStore } from './stores/usePlayerStore';
import YouTube, { YouTubeEvent } from 'react-youtube';
import { IPlayer } from './Player';

export const YouTubePlayer: IPlayer = (props) => {
	// Prevents infinite rerenders
	// https://docs.pmnd.rs/zustand/recipes/recipes#transient-updates-(for-frequent-state-changes)
	const [setActive, unload] = usePlayerStore((set) => [set.setActive, set.unload]);
	const setPlayerApi = React.useRef(usePlayerStore.getState().setPlayerApi);
	const playerElementRef = React.useRef<IPlayerApi | undefined>(undefined);

	const loadVideo = (id: string) => {};

	React.useEffect(() => {
		return () => {
			setPlayerApi.current(undefined);
			unload();
		};
	}, []);

	const onReady = (event: YouTubeEvent<any>): void => {
		const player = event.target;
		playerElementRef.current = {
			loadVideo,
			play() {
				player.playVideo();
			},
			pause() {
				player.pauseVideo();
			},
		};
		setPlayerApi.current(playerElementRef);
	};

	return (
		<YouTube
			opts={{
				width: '100%',
				height: '100%',
				playerVars: {
					controls: 0,
					modestbranding: 1,
				},
			}}
			style={{
				height: '100%',
				width: '100%',
			}}
			videoId={props.videoId}
			onReady={onReady}
			onPause={() => setActive(false)}
			onPlay={() => {
				setPlayerApi.current(playerElementRef);
				setActive(true);
			}}
		/>
	);
};

