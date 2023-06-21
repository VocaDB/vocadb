import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Carousel } from '@mantine/carousel';
import { HighlightedSongCard } from './HighlightedSongCard';
import { useMantineTheme } from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';

interface HighlightedSongsCarouselProps {
	songs: SongWithPVAndVoteContract[];
}

export function HighlightedSongsCarousel({ songs }: HighlightedSongsCarouselProps) {
	const theme = useMantineTheme();
	const isMobile = useMediaQuery(`(max-width: ${theme.breakpoints['lg']})`);

	return (
		<>
			<div style={{ display: 'flex' }}>
				<Carousel
					h={'100%'}
					sx={{ flex: 1, maxWidth: '100%' }}
					align={isMobile ? 'center' : 'start'}
					slideSize="25%"
					slideGap="md"
					breakpoints={[
						{ maxWidth: 'lg', slideSize: '50%', slideGap: 'md' },
						{ maxWidth: 'xs', slideSize: '95%', slideGap: '5%' },
					]}
					loop
					draggable={isMobile}
					previousControlLabel="Previous highlighted PVs"
					nextControlLabel="Next hightlighted PV"
				>
					{songs.map((song, key) => (
						<Carousel.Slide key={key}>
							<HighlightedSongCard
								song={song}
								priority={isMobile ? key < 2 || key == songs.length - 1 : key < 4}
							/>
						</Carousel.Slide>
					))}
				</Carousel>
			</div>
		</>
	);
}

