import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Carousel, CarouselSlide } from '@mantine/carousel';
import { HighlightedSongCard } from './HighlightedSongCard';
import '@mantine/carousel/styles.css';

interface HighlightedSongsCarouselProps {
	songs: SongWithPVAndVoteContract[];
}

export default function HighlightedSongsCarousel({ songs }: HighlightedSongsCarouselProps) {
	// const theme = useMantineTheme();
	// const isMobile = useMediaQuery(`(max-width: ${theme.breakpoints['lg']})`);

	return (
		<div style={{ display: 'flex' }}>
			<Carousel
				height={'100%'}
				style={{ flex: 1, maxWidth: '100%' }}
				align={'center'}
				// slideSize="25%"
				// slideGap="md"
				slideGap={{ base: '5%', xs: 'md' }}
				slideSize={{ base: '95%', xs: '50%', lg: '25%' }}
				// breakpoints={[
				// 	{ maxWidth: 'lg', slideSize: '50%', slideGap: 'md' },
				// 	{ maxWidth: 'xs', slideSize: '95%', slideGap: '5%' },
				// ]}
				loop
				draggable={true}
				// previousControlLabel="Previous highlighted PVs"
				// nextControlLabel="Next hightlighted PV"
			>
				{songs.map((song, key) => (
					<CarouselSlide key={key}>
						<HighlightedSongCard
							song={song}
							// priority={isMobile ? key < 2 || key == songs.length - 1 : key < 4}
							priority={key === 0 || key === -1 || key === 1}
						/>
					</CarouselSlide>
				))}
			</Carousel>
		</div>
	);
}

