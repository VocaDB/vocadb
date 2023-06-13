import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Carousel } from '@mantine/carousel';
import { HighlightedSongCard } from './HighlightedSongCard';
import { Grid, MediaQuery } from '@mantine/core';

interface HighlightedSongsCarouselProps {
	songs: SongWithPVAndVoteContract[];
}

export function HighlightedSongsCarousel({ songs }: HighlightedSongsCarouselProps) {
	return (
		<>
			<MediaQuery smallerThan="lg" styles={{ display: 'none' }}>
				<Grid>
					{songs.slice(0, 4).map((s, index) => (
						<Grid.Col span={3} key={index}>
							<HighlightedSongCard song={s} priority />
						</Grid.Col>
					))}
				</Grid>
			</MediaQuery>

			<div style={{ display: 'flex' }}>
				<MediaQuery largerThan="lg" styles={{ display: 'none' }}>
					<Carousel
						h={'100%'}
						sx={{ flex: 1, maxWidth: '100%' }}
						align="center"
						slideSize="50%"
						slideGap="md"
						breakpoints={[{ maxWidth: 'xs', slideSize: '95%', slideGap: '5%' }]}
						loop
						previousControlLabel="Previous highlighted PVs"
						nextControlLabel="Next hightlighted PV"
					>
						{songs.map((song, key) => (
							<Carousel.Slide key={key}>
								<HighlightedSongCard song={song} priority={key < 2} />
							</Carousel.Slide>
						))}
					</Carousel>
				</MediaQuery>
			</div>
		</>
	);
}

