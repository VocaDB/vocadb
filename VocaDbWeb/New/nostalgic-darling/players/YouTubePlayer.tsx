import { useEffect, useRef } from 'react';
import { IPlayerApi, usePlayerStore } from '../stores/usePlayerStore';
import YouTube, { YouTubeEvent, YouTubePlayer as YT } from 'react-youtube';
import { IPlayer } from '../Player';

export const YouTubePlayer: IPlayer = ({ pv }) => {
	const [setActive, onEnd, setPlayerApi, volume] = usePlayerStore((set) => [
		set.setActive,
		set.onEnd,
		set.setPlayerApi,
		set.volume,
	]);
	const playerElementRef = useRef<YT | undefined>(undefined);
	const playerApiRef = useRef<IPlayerApi | undefined>(undefined);

	useEffect(() => {
		return () => {
			setPlayerApi(undefined);
		};
	}, []);

	const onReady = (event: YouTubeEvent<any>): void => {
		const player = event.target;
		playerElementRef.current = player;
		player.setVolume(volume);
		playerApiRef.current = {
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
			setCurrentTime(newProgress) {
				return player.seekTo(newProgress);
			},
		};
		setPlayerApi(playerApiRef.current);
	};

	useEffect(() => {
		const player = playerElementRef.current;
		if (player !== undefined) {
			playerElementRef.current.setVolume(volume);
		}
	}, [volume]);

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
			videoId={pv.pvId}
			onReady={onReady}
			onPause={() => setActive(false)}
			onPlay={() => {
				setPlayerApi(playerApiRef.current);
				setActive(true);
			}}
			onEnd={onEnd}
		/>
	);
};

