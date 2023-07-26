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

	const interval = useInterval(() => {
		if (playerApi === undefined) return;
		setProgress(playerApi.getCurrentTime() / playerApi.getDuration());
		setDuration(playerApi.getDuration());
	}, 500);

	useEffect(() => {
		interval.start();
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
				<ActionIcon
					title="Pause"
					onClick={(e) => {
						e.stopPropagation();
						playerApi?.pause();
					}}
				>
					<IconPlayerPause />
				</ActionIcon>
			) : (
				<ActionIcon
					title="Play"
					onClick={(e) => {
						e.stopPropagation();
						playerApi?.play();
					}}
				>
					<IconPlayerPlay />
				</ActionIcon>
			)}
			{!showMobileLayout && (
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
							if (playerApi === undefined) return;
							playerApi.setCurrentTime((progress / 100) * playerApi.getDuration());
						}}
					/>
					<Text size="sm" color="dimmed">
						{formatNumberToTime(duration)}
					</Text>
				</div>
			)}
		</div>
	);
}

