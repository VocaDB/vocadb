import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Carousel } from '@mantine/carousel';
import { HighlightedSongCard } from './HighlightedSongCard';

interface HighlightedSongsCarouselProps {
	songs: SongWithPVAndVoteContract[];
}

export function HighlightedSongsCarousel({ songs }: HighlightedSongsCarouselProps) {
	return (
		<Carousel
			w={'100%'}
			align="start"
			slideSize="33.333333%"
			slideGap="md"
			breakpoints={[{ maxWidth: 'lg', slideSize: '100%', slideGap: 0 }]}
			loop
			withIndicators
		>
			{songs.concat(songs).map((song, key) => (
				<Carousel.Slide key={key}>
					<HighlightedSongCard song={song} priority={key === 0} />
				</Carousel.Slide>
			))}
		</Carousel>
	);
}

