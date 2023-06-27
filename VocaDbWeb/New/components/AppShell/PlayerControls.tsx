import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { ActionIcon, Group, Slider, Text } from '@mantine/core';
import { useInterval } from '@mantine/hooks';
import { IconPlayerPause, IconPlayerPlay } from '@tabler/icons-react';
import React from 'react';
import { useRef, useState } from 'react';

// Formats 160 to 2:40
const formatNumberToTime = (number: number): string => {
	const rounded = Math.round(number);
	const minutes = Math.floor(rounded / 60);
	const remaining = rounded - 60 * minutes;
	return `${minutes}:${(remaining < 10 ? '0' : '') + remaining}`;
};

export default function PlayerControls() {
	const [song, active, playerApi] = usePlayerStore((state) => [
		state.song,
		state.active,
		state.playerApi,
	]);

	const [duration, setDuration] = useState(0);
	const [progress, setProgress] = useState(0);
	// https://github.com/mantinedev/mantine/issues/2840
	const currentState = useRef(playerApi);
	currentState.current = playerApi;

	const interval = useInterval(() => {
		if (playerApi === undefined) return;
		setProgress(playerApi.getCurrentTime() / playerApi.getDuration());
	}, 500);

	React.useEffect(() => {
		interval.start();
		setDuration(playerApi?.getDuration() ?? 0);
		return interval.stop();
	}, [playerApi]);

	if (typeof window === 'undefined' || song === undefined) {
		return <></>;
	}

	return (
		<div
			style={{
				display: 'flex',
				flexDirection: 'column',
				justifyContent: 'center',
				alignItems: 'center',
				width: '25vw',
			}}
		>
			{active ? (
				<ActionIcon onClick={() => playerApi?.pause()}>
					<IconPlayerPause />
				</ActionIcon>
			) : (
				<ActionIcon onClick={() => playerApi?.play()}>
					<IconPlayerPlay />
				</ActionIcon>
			)}
			<div
				style={{
					display: 'flex',
					flexDirection: 'row',
					alignItems: 'center',
					justifyContent: 'space-between',
					width: '100%',
				}}
			>
				<Text size="sm" color="dimmed">
					{formatNumberToTime(progress * duration)}
				</Text>
				<Slider
					w="70%"
					size="sm"
					value={progress * 100}
					showLabelOnHover={false}
					onChange={(newProgress) => {
						if (interval.active) interval.stop();
						setProgress(newProgress / 100);
					}}
					label={null}
					onChangeEnd={(progress) => {
						interval.start();
						if (currentState.current === undefined) return;
						currentState.current.setCurrentTime(
							(progress / 100) * currentState.current.getDuration()
						);
					}}
				/>
				<Text size="sm" color="dimmed">
					{formatNumberToTime(duration)}
				</Text>
			</div>
		</div>
	);
}

