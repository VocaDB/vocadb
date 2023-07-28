import { FocusTrap, Paper, ScrollArea, Text, useMantineTheme } from '@mantine/core';
import { usePlayerStore } from './stores/usePlayerStore';
import { useEffect, useState } from 'react';
import { LyricsForSongContract } from '@/types/DataContracts/Song/LyricsForSongContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { useStyles } from './LyricsContainer.styles';

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
			<div className={styles.classes.divWrapper}>
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

