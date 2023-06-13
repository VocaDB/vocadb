import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Carousel } from '@mantine/carousel';
import { HighlightedSongCard } from './HighlightedSongCard';

interface HighlightedSongsCarouselProps {
	songs: SongWithPVAndVoteContract[];
}

export function HighlightedSongsCarousel({ songs }: HighlightedSongsCarouselProps) {
	return (
		<div style={{ display: 'flex' }}>
			<Carousel
				h={'100%'}
				sx={{ flex: 1, maxWidth: '100%' }}
				align="center"
				slideSize="25%"
				slideGap="md"
				breakpoints={[
					{ maxWidth: 'xl', slideSize: '33.33333%' },
					{ maxWidth: 'md', slideSize: '50%' },
					{ maxWidth: 'xs', slideSize: '95%', slideGap: '5%' },
				]}
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
		</div>
	);
}

