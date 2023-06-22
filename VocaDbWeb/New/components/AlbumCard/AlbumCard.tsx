import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, Title } from '@mantine/core';
import { AlbumToolTip } from '../ToolTips/AlbumToolTip';
import useStyles from './AlbumCard.styles';
import CustomImage from '../Image/Image';

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
						display: 'flex',
						flexDirection: 'column',
						alignItems: 'center',
					}}
				>
					<div style={{ overflow: 'hidden' }}>
						<CustomImage
							src={album.mainPicture?.urlOriginal ?? '/unknown.png'}
							className={styles.classes.image}
							width={200}
							height={200}
							mode="crop"
							alt="Preview"
						/>
					</div>
					<Title style={{ width: '200px' }} mt="xs" order={5}>
						{album.name}
					</Title>
					<Title style={{ width: '200px' }} order={6} color="dimmed">
						{album.artistString.split('feat.')[0]}
					</Title>
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
		<Grid>
			{albums.slice(0, 6).map((album) => (
				<Grid.Col xl={2} sm={12} key={album.id}>
					<AlbumCard key={album.id} album={album} />
				</Grid.Col>
			))}
		</Grid>
	);
}

