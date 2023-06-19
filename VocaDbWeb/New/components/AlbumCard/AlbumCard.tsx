import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, MediaQuery, Text } from '@mantine/core';
import Image from 'next/image';
import useStyles from './AlbumCard.styles';
import { AlbumToolTip } from '../ToolTips/AlbumToolTip';

interface AlbumCardProps {
	album: AlbumForApiContract;
}

export function AlbumCard({ album }: AlbumCardProps) {
	return (
		<>
			<AlbumToolTip album={album}>
				<Image
					src={album.mainPicture?.urlSmallThumb ?? '/unknown.png'}
					width={150}
					height={150}
					alt="Preview"
				/>
			</AlbumToolTip>
			<Text>{album.name}</Text>
		</>
	);
}

interface AlbumCardsProps {
	albums: AlbumForApiContract[];
}

export function AlbumCards({ albums }: AlbumCardsProps) {
	const styles = useStyles();
	return (
		<Grid>
			{albums.slice(0, 4).map((album, index) => (
				<Grid.Col xl={3} md={4} xs={6} key={album.id}>
					<MediaQuery
						smallerThan="xl"
						largerThan="md"
						styles={index === 3 ? { display: 'none' } : {}}
					>
						<div>
							<div className={styles.classes.cardWrapper}>
								<AlbumCard key={album.id} album={album} />
							</div>
						</div>
					</MediaQuery>
				</Grid.Col>
			))}
		</Grid>
	);
}

