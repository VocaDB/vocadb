import React from 'react';
import { IPlayerApi, usePlayerStore } from './stores/usePlayerStore';
import YouTube, { YouTubeEvent } from 'react-youtube';
import { IPlayer } from './Player';

export const YouTubePlayer: IPlayer = (props) => {
	const [setActive, onEnd, setPlayerApi] = usePlayerStore((set) => [
		set.setActive,
		set.onEnd,
		set.setPlayerApi,
	]);
	const playerElementRef = React.useRef<IPlayerApi | undefined>(undefined);

	React.useEffect(() => {
		return () => {
			setPlayerApi(undefined);
		};
	}, []);

	const onReady = (event: YouTubeEvent<any>): void => {
		const player = event.target;
		playerElementRef.current = {
			play() {
				player.playVideo();
			},
			pause() {
				player.pauseVideo();
			},
			getCurrentTime() {
				return player.getCurrentTime();
			},
			getDuration() {
				return player.getDuration();
			},
		};
		setPlayerApi(playerElementRef.current);
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
				setPlayerApi(playerElementRef.current);
				setActive(true);
			}}
			onEnd={onEnd}
		/>
	);
};

