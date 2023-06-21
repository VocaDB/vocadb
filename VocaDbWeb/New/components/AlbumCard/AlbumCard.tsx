import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, Title } from '@mantine/core';
import Image from 'next/image';
import { AlbumToolTip } from '../ToolTips/AlbumToolTip';
import useStyles from './AlbumCard.styles';

interface AlbumCardProps {
	album: AlbumForApiContract;
}

export function AlbumCard({ album }: AlbumCardProps) {
	const styles = useStyles();
	return (
		<>
			<AlbumToolTip album={album}>
				<div
					style={{
						width: '100%',
						height: '250px',
						position: 'relative',
						display: 'flex',
						justifyContent: 'center',
					}}
				>
					<div style={{ overflow: 'hidden' }}>
						<Image
							src={album.mainPicture?.urlOriginal ?? '/unknown.png'}
							className={styles.classes.image}
							width={250}
							height={250}
							alt="Preview"
							priority
						/>
					</div>
					<div className={styles.classes.cardContent}>
						<div style={{ color: 'white' }}>
							<Title mx="md" order={3}>
								{album.name}
							</Title>
							<Title mb="sm" mx="md" c="dimmed" order={4}>
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
	return (
		<Grid style={{ maxWidth: '532px' }}>
			{albums.slice(0, 4).map((album) => (
				<Grid.Col xl={6} sm={12} key={album.id}>
					<AlbumCard key={album.id} album={album} />
				</Grid.Col>
			))}
		</Grid>
	);
}

