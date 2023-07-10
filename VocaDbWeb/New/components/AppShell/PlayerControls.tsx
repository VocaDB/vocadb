import { formatNumberToTime } from '@/Helpers/DateTimeHelper';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { ActionIcon, Slider, Text } from '@mantine/core';
import { useInterval } from '@mantine/hooks';
import { IconPlayerPause, IconPlayerPlay } from '@tabler/icons-react';
import React, { useEffect } from 'react';
import { useRef, useState } from 'react';

interface PlayerControlsProps {
	showMobileLayout?: boolean;
}

export default function PlayerControls({ showMobileLayout }: PlayerControlsProps) {
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
		setDuration(playerApi.getDuration());
	}, 500);

	useEffect(() => {
		interval.start();
		currentState.current = playerApi;
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
				alignItems: showMobileLayout ? 'flex-end' : 'center',
				width: '25vw',
			}}
		>
			{active ? (
				<ActionIcon title="Pause" onClick={() => playerApi?.pause()}>
					<IconPlayerPause />
				</ActionIcon>
			) : (
				<ActionIcon title="Play" onClick={() => playerApi?.play()}>
					<IconPlayerPlay />
				</ActionIcon>
			)}
			<div
				style={{
					display: showMobileLayout ? 'none' : 'flex',
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
					title="Song progress slider"
					thumbLabel="Song progress slider thumb"
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

