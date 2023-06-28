import { Group, Paper, Slider, Text, createStyles, rem } from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import React, { useRef } from 'react';
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
	const [playerApi, volume, setVolume] = usePlayerStore((set) => [
		set.playerApi,
		set.volume,
		set.setVolume,
	]);
	// https://github.com/mantinedev/mantine/issues/2840
	const currentState = useRef(playerApi);
	currentState.current = playerApi;

	return (
		<Group>
			<IconVolume color="gray" />
			<Slider
				w="10vw"
				value={volume}
				label={Math.round(volume)}
				onChange={(newVolume) => {
					setVolume(newVolume);
				}}
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
		height: '100%',
	},
}));

const CustomFooter = () => {
	const styles = useStyles();
	return (
		<div className={styles.classes.base}>
			<Paper mx="sm" className={styles.classes.footer} component="footer">
				{/* TODO: Use flex container instead of Center */}
				<SongInfo />
				<PlayerControls />
				<VolumeControl />
			</Paper>
		</div>
	);
};

export default CustomFooter;

