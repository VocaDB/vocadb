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
import React, { useRef, useState } from 'react';
import { IconMicrophone2, IconVolume, IconVolumeOff } from '@tabler/icons-react';
import { useMediaQuery, usePrevious } from '@mantine/hooks';
import { motion } from 'framer-motion';

interface SongInfoProps {
	showMobileLayout?: boolean;
}

// Displays the song info on the left side of the footer
const SongInfo = ({ showMobileLayout }: SongInfoProps) => {
	const [song] = usePlayerStore((state) => [state.song]);

	if (song === undefined) {
		return <></>;
	}

	return (
		<div
			style={{
				marginTop: 'auto',
				marginBottom: 'auto',
				maxWidth: showMobileLayout ? '80%' : '30%',
			}}
		>
			<Text truncate>{song.name}</Text>
			<Text color="dimmed" truncate>
				{song.artistString}
			</Text>
		</div>
	);
};

const VolumeControl = () => {
	const [playerApi, volume, setVolume, lyricsAvailable, showLyrics, toggleLyrics] =
		usePlayerStore((set) => [
			set.playerApi,
			set.volume,
			set.setVolume,
			set.lyricsAvailable,
			set.showLyrics,
			set.toggleLyrics,
		]);
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
				disabled={!lyricsAvailable}
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
	const theme = useMantineTheme();
	const styles = useStyles();
	const isMobile = useMediaQuery(`(max-width: ${theme.breakpoints['sm']})`);
	const [expanded, setExpanded] = useState(false);

	// TODO: Maybe move this check to Footer component
	if (typeof window === 'undefined') {
		return <></>;
	}

	return (
		<div className={styles.classes.base}>
			<Paper
				onClick={() => setExpanded(isMobile && !expanded)}
				px="sm"
				className={styles.classes.footer}
				component="footer"
			>
				<SongInfo showMobileLayout={isMobile} />
				<PlayerControls showMobileLayout={isMobile} />
				{!isMobile && <VolumeControl />}
			</Paper>
			<Paper
				component={motion.div}
				onClick={() => setExpanded(isMobile && !expanded)}
				bg="blue"
				animate={{
					position: 'fixed',
					top: expanded ? 50 : '100vh',
					bottom: 64,
					width: '100%',
					zIndex: 100,
				}}
			>
				<p>Text</p>
			</Paper>
		</div>
	);
};

export default CustomFooter;

