import { Center, Group, Paper, Slider, Text, createStyles, rem } from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { useInterval } from '@mantine/hooks';
import React, { useRef, useState } from 'react';
import { IconVolume } from '@tabler/icons-react';

// Displays the song info on the left side of the footer
const SongInfo = () => {
	const [song] = usePlayerStore((state) => [state.song]);

	// TODO: Maybe move this check to Footer component
	if (typeof window === 'undefined' || song === undefined) {
		return <></>;
	}

	return (
		<div style={{ marginTop: 'auto', marginBottom: 'auto' }}>
			<Text>{song.name}</Text>
			<Text color="dimmed">{song.artistString}</Text>
		</div>
	);
};

const VolumeControl = () => {
	const [playerApi] = usePlayerStore((set) => [set.playerApi]);
	const [volume, setVolume] = useState(100);
	// https://github.com/mantinedev/mantine/issues/2840
	const currentState = useRef(playerApi);
	currentState.current = playerApi;

	const interval = useInterval(() => {
		if (playerApi === undefined) return;
		setVolume(playerApi.getVolume());
	}, 1000);

	React.useEffect(() => {
		interval.start();
		return interval.stop();
	}, [playerApi]);

	return (
		<Group>
			<IconVolume color="gray" />
			<Slider
				w="10vw"
				value={volume}
				onChange={(newVolume) => {
					if (interval.active) interval.stop();
					if (currentState.current === undefined) return;
					currentState.current.setVolume(newVolume);
					setVolume(newVolume);
				}}
				onChangeEnd={() => interval.start()}
			/>
		</Group>
	);
};

const useStyles = createStyles((theme) => ({
	base: {
		position: 'fixed',
		height: 65,
		right: 0,
		bottom: 0,
		left: 300,
		[theme.fn.smallerThan('lg')]: {
			left: 220,
		},
		[theme.fn.smallerThan('sm')]: {
			left: 0,
		},
		borderTop: `${rem(1)} solid ${
			theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]
		}`,
	},
	footer: {
		display: 'flex',
		flexDirection: 'row',
		justifyContent: 'space-between',
		alignContent: 'center',
		height: '100%',
	},
}));

const CustomFooter = () => {
	const [playerApi] = usePlayerStore((set) => [set.playerApi]);
	const styles = useStyles();
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
		return interval.stop();
	}, [playerApi]);

	return (
		<div className={styles.classes.base}>
			<Paper mx="sm" className={styles.classes.footer} component="footer">
				{/* <Slider
					styles={() => ({
						trackContainer: {},
					})}
					w="100%"
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
				/> */}
				{/* TODO: Use flex container instead of Center */}
				<SongInfo />
				<PlayerControls />
				<VolumeControl />
			</Paper>
		</div>
	);
};

export default CustomFooter;

