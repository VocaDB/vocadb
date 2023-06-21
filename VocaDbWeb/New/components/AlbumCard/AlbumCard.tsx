import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, MediaQuery, Text, Title } from '@mantine/core';
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
				<div style={{ position: 'relative' }}>
					<Image
						src={album.mainPicture?.urlThumb ?? '/unknown.png'}
						width={250}
						height={250}
						alt="Preview"
					/>
					<div
						style={{
							position: 'absolute',
							display: 'flex',
							flexDirection: 'column',
							justifyContent: 'flex-end',
							top: 0,
							width: '100%',
							height: '100%',
							background: 'linear-gradient(to top, rgba(0, 0, 0, 0.70), transparent)',
						}}
					>
						<div style={{ color: 'white' }}>
							<Title mx="md" order={3}>
								{album.name}
							</Title>
							<Title mb="md" mx="md" c="dimmed" order={4}>
								{album.artistString.split('feat.')[0]}
							</Title>
						</div>
					</div>
				</div>
			</AlbumToolTip>
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

