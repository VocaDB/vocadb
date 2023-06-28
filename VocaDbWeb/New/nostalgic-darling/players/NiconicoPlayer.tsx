import React, { useRef } from 'react';
import { IPlayer } from '../Player';
import { IPlayerApi, usePlayerStore } from '../stores/usePlayerStore';

// TODO: Move these into a separate file
export interface PlayerStatusEvent {
	eventName: 'playerStatusChange';
	data: {
		playerStatus: PlayerStatus;
	};
}

export interface StatusEvent {
	eventName: 'statusChange';
	data: {
		playerStatus: PlayerStatus;
	};
}

export interface MetadataEvent {
	eventName: 'playerMetadataChange';
	data: {
		currentTime: number;
		duration: number;
		volume: number;
	};
}

export interface LoadCompleteEvent {
	eventName: 'loadComplete';
	data: {
		videoInfo: {
			watchId: string;
			lengthInSeconds: number;
		};
	};
}

type EventData = PlayerStatusEvent | StatusEvent | MetadataEvent | LoadCompleteEvent;

export interface PlayerEvent {
	origin: string;
	data: EventData;
}

enum PlayerStatus {
	Play = 2,
	Pause = 3,
	End = 4,
}
const NICO_ORIGIN = 'https://embed.nicovideo.jp';

export const NiconicoPlayer: IPlayer = ({ pv }) => {
	const [setActive, onEnd, setPlayerApi] = usePlayerStore((set) => [
		set.setActive,
		set.onEnd,
		set.setPlayerApi,
	]);
	const durationRef = useRef(0);
	const currentTimeRef = useRef(0);
	const volumeRef = useRef(0);

	const playerApiRef = useRef<IPlayerApi | undefined>(undefined!);
	const playerElementRef = useRef<HTMLIFrameElement>(undefined!);

	const postMessage = (message: any): void => {
		playerElementRef.current.contentWindow?.postMessage(
			{
				...message,
				playerId: '1' /* Needs to be a string, not a number. */,
				sourceConnectorType: 1,
			},
			NICO_ORIGIN
		);
	};

	const handleMessage = (e: PlayerEvent): void => {
		if (e.origin !== NICO_ORIGIN) return;

		const data = e.data;

		switch (data.eventName) {
			case 'playerStatusChange':
				break;

			case 'statusChange':
				switch (data.data.playerStatus) {
					case PlayerStatus.Play:
						setActive(true);
						break;

					case PlayerStatus.Pause:
						setActive(false);
						break;

					case PlayerStatus.End:
						onEnd();
						break;
				}
				break;

			case 'playerMetadataChange':
				if (data.data.duration !== undefined) {
					durationRef.current = data.data.duration / 1000;
				}

				currentTimeRef.current =
					data.data.currentTime === undefined ? 0 : data.data.currentTime / 1000;

				volumeRef.current = data.data.volume * 100;

				break;

			case 'loadComplete':
				durationRef.current = data.data.videoInfo.lengthInSeconds;

				playerApiRef.current = {
					play() {
						postMessage({ eventName: 'play' });
					},
					pause() {
						postMessage({ eventName: 'pause' });
					},
					getCurrentTime() {
						return currentTimeRef.current;
					},
					getDuration() {
						return durationRef.current;
					},
					setCurrentTime(newProgress) {
						postMessage({ eventName: 'seek', data: { time: newProgress * 1000 } });
					},
					setVolume(volume) {
						postMessage({
							eventName: 'volumeChange',
							data: { volume: volume / 100 },
						});
					},
					getVolume() {
						return volumeRef.current;
					},
				};
				setPlayerApi(playerApiRef.current);
				break;

			default:
				break;
		}
	};

	React.useEffect(() => {
		window.addEventListener('message', handleMessage);
		return () => {
			window.removeEventListener('message', handleMessage);
			setPlayerApi(undefined);
		};
	}, []);

	return (
		<iframe
			ref={playerElementRef}
			src={`https://embed.nicovideo.jp/watch/${pv.pvId}?jsapi=1&playerId=1`}
			width="100%"
			height="100%"
			allowFullScreen
			style={{ border: 'none' }}
			// The player has to have the allow="autoplay" attribute.
			// Otherwise it throws a NotAllowedError: "play() failed because the user didn't interact with the document first".
			// See also: https://github.com/vimeo/player.js/issues/389.
			// NOTE: An iframe element created by `PVPlayerNiconico.playerFactory.create` doesn't have the allow="autoplay" attribute,
			// which causes the above issue when trying to autoplay a video.
			allow="autoplay; fullscreen"
		/>
	);
};

