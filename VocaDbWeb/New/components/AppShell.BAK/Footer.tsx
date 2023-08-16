import { ActionIcon, Group, Paper, Slider, Text } from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import React, { useState } from 'react';
import { IconMicrophone2, IconVolume, IconVolumeOff } from '@tabler/icons-react';
import { useMediaQuery, usePrevious } from '@mantine/hooks';
import MobilePlayer from './MobilePlayer';
import { useStyles } from './Footer.styles';

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
	const [volume, setVolume, lyricsAvailable, showLyrics, toggleLyrics] = usePlayerStore((set) => [
		set.volume,
		set.setVolume,
		set.lyricsAvailable,
		set.showLyrics,
		set.toggleLyrics,
	]);
	const lastVolume = usePrevious(volume);

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

const CustomFooter = () => {
	const { classes, theme } = useStyles();
	const [expanded, setExpanded] = useState(false);
	const isMobile = useMediaQuery(`(max-width: ${theme.breakpoints['sm']})`);

	return (
		<div className={classes.base}>
			<Paper
				onClick={() => setExpanded(isMobile && !expanded)}
				px="md"
				className={classes.footer}
				component="footer"
			>
				<SongInfo showMobileLayout={isMobile} />
				<PlayerControls showMobileLayout={isMobile} />
				{!isMobile && <VolumeControl />}
			</Paper>
			<MobilePlayer expanded={expanded} onClose={() => setExpanded(false)} />
		</div>
	);
};

export default CustomFooter;

