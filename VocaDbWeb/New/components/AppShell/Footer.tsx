import {
	ActionIcon,
	Group,
	Paper,
	Slider,
	Text,
	createStyles,
	rem,
	useMantineTheme,
} from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import React, { useRef } from 'react';
import { IconMicrophone2, IconVolume, IconVolumeOff } from '@tabler/icons-react';
import { usePrevious } from '@mantine/hooks';

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
	const [playerApi, volume, setVolume, showLyrics, toggleLyrics] = usePlayerStore((set) => [
		set.playerApi,
		set.volume,
		set.setVolume,
		set.showLyrics,
		set.toggleLyrics,
	]);
	const theme = useMantineTheme();
	const lastVolume = usePrevious(volume);
	// https://github.com/mantinedev/mantine/issues/2840
	const currentState = useRef(playerApi);
	currentState.current = playerApi;

	return (
		<Group spacing="xs">
			<ActionIcon
				title={showLyrics ? 'Hide Lyrics' : 'Show lyrics'}
				color={showLyrics ? 'default' : undefined}
				onClick={toggleLyrics}
				size="sm"
			>
				<IconMicrophone2 />
			</ActionIcon>
			<ActionIcon
				size="sm"
				title={volume > 0 ? 'Mute' : 'Unmute'}
				onClick={() => {
					if (volume > 0) {
						setVolume(0);
					} else {
						setVolume(lastVolume ?? 100);
					}
				}}
			>
				{volume > 0 ? <IconVolume /> : <IconVolumeOff />}
			</ActionIcon>
			<Slider
				w="10vw"
				size="sm"
				value={volume}
				title="Song volume"
				thumbLabel="Song volume thumb"
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
			<Paper px="sm" className={styles.classes.footer} component="footer">
				{/* TODO: Use flex container instead of Center */}
				<SongInfo />
				<PlayerControls />
				<VolumeControl />
			</Paper>
		</div>
	);
};

export default CustomFooter;

