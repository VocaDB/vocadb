import { useEffect, useRef } from 'react';
import { IPlayer } from '../Player';
import { usePlayerStore } from '../stores/usePlayerStore';

export const BilibiliPlayer: IPlayer = ({ pv }) => {
	const [setActive, onEnd, setPlayerApi, volume] = usePlayerStore((set) => [
		set.setActive,
		set.onEnd,
		set.setPlayerApi,
		set.volume,
	]);

	const videoRef = useRef<HTMLVideoElement>(undefined!);
	const audioRef = useRef<HTMLAudioElement>(undefined!);

	const onLoad = () => {
		const player = videoRef.current;
		audioRef.current.volume = volume / 100;
		setPlayerApi({
			play() {
				player.play();
			},
			pause() {
				player.pause();
			},
			getCurrentTime() {
				return player.currentTime;
			},
			getDuration() {
				return player.duration;
			},
			setCurrentTime(newProgress) {
				player.currentTime = newProgress;
			},
		});
	};

	useEffect(() => {
		audioRef.current.volume = volume / 100;
	}, [volume]);

	useEffect(() => {
		onLoad();

		return () => setPlayerApi(undefined);
	}, []);

	return (
		<>
			<video
				ref={videoRef}
				onPlay={() => {
					audioRef.current.play();
					setActive(true);
				}}
				onPause={() => audioRef.current.pause()}
				onSeeked={() => (audioRef.current.currentTime = videoRef.current.currentTime)}
				onEnded={() => onEnd()}
				controls
				width="100%"
				height="100%"
			>
				<source
					src={`https://dream-traveler.fly.dev/bilibili/video?url=${pv.url}`}
					type="video/mp4"
				/>
			</video>
			<audio ref={audioRef} style={{ display: 'none' }}>
				<source
					src={`https://dream-traveler.fly.dev/bilibili/audio?url=${pv.url}`}
					type="video/mp4"
				/>
			</audio>
		</>
		// <iframe
		// 	onLoad={onLoad}
		// 	style={{ width: '100%', height: '100%' }}
		// 	src={`//player.bilibili.com/player.html?aid=${props.videoId}&page=1`}
		// />
	);
};

