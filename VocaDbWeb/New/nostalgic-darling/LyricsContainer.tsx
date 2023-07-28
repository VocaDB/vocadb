import { FocusTrap, Paper, ScrollArea, Text, createStyles, useMantineTheme } from '@mantine/core';
import { usePlayerStore } from './stores/usePlayerStore';
import { useEffect, useState } from 'react';
import { LyricsForSongContract } from '@/types/DataContracts/Song/LyricsForSongContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';

// TODO: Move this in a separate file
const useStyles = createStyles((theme) => ({
	lyricsContainer: {
		display: 'grid',
		justifyContent: 'center',
	},
	lyricsWrapper: {
		maxWidth: 'max-content',
		[theme.fn.smallerThan('sm')]: {
			marginLeft: theme.spacing.sm,
			marginRight: theme.spacing.sm,
		},
	},
	lyricLine: {
		lineHeight: '1.5em',
		fontSize: '2rem',
		fontWeight: 700,
		color: 'white',
		[theme.fn.smallerThan('lg')]: {
			fontSize: '1.5rem',
		},
	},
}));

export default function LyricsContainer() {
	const theme = useMantineTheme();
	const [song, showLyrics, setLyricsAvailable] = usePlayerStore((set) => [
		set.song,
		set.showLyrics,
		set.setLyricsAvailable,
	]);
	const styles = useStyles();
	const [lyrics, setLyrics] = useState<LyricsForSongContract | undefined>();

	useEffect(() => {
		if (song !== undefined) {
			// TOOD: Add lyrics to SongApiContract and change this to apiGet
			apiFetch(`/api/songs/${song.id}?fields=Lyrics`)
				.then((resp) => resp.json())
				.then((resp) => resp.lyrics as LyricsForSongContract[])
				.then((lyrics) => lyrics.filter((l) => l.translationType === 'Original')[0])
				.then((lyrics) => setLyrics(lyrics));
		}
	}, [song]);

	useEffect(() => {
		setLyricsAvailable(lyrics !== undefined);
	}, [lyrics]);

	if (!showLyrics || lyrics === undefined) {
		return <></>;
	}

	return (
		<FocusTrap>
			<div
				style={{
					position: 'absolute',
					top: 0,
					left: 0,
					height: '100%',
					width: '100%',
					zIndex: 100,
				}}
			>
				<Paper component={ScrollArea} h="100%" bg={theme.colors[theme.primaryColor][7]}>
					<div className={styles.classes.lyricsContainer}>
						<div className={styles.classes.lyricsWrapper}>
							{lyrics.value
								?.split('\n')
								.map((line) =>
									line === '' || line === '\r' ? (
										<br />
									) : (
										<Text className={styles.classes.lyricLine}>{line}</Text>
									)
								)}
						</div>
					</div>
				</Paper>
			</div>
		</FocusTrap>
	);
}

