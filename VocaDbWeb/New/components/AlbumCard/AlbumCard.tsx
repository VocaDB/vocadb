import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, GridCol, Title, UnstyledButton } from '@mantine/core';
import CustomImage from '../Image/Image';
import EntryToolTip from '../ToolTips/EntryToolTip';
import styles from './AlbumCard.module.css';

interface AlbumCardProps {
	album: AlbumForApiContract;
}

export function AlbumCard({ album }: AlbumCardProps) {
	return (
		// <EntryToolTip entry="album" album={album}>
		<UnstyledButton
			style={{
				display: 'flex',
				flexDirection: 'column',
				alignItems: 'center',
				width: '100%',
			}}
		>
			<div style={{ overflow: 'hidden' }}>
				<CustomImage
					src={album.mainPicture?.urlOriginal ?? '/unknown.png'}
					className={styles.image}
					width={188}
					height={188}
					mode="crop"
					alt="Preview"
				/>
			</div>
			<Title style={{ width: '180px' }} mt="xs" order={5}>
				{album.name}
			</Title>
			<Title style={{ width: '180px' }} order={6} color="dimmed">
				{album.artistString.split('feat.')[0]}
			</Title>
		</UnstyledButton>
		// </EntryToolTip>
	);
}

interface AlbumCardsProps {
	albums: AlbumForApiContract[];
}

export function AlbumCards({ albums }: AlbumCardsProps) {
	return (
		<Grid>
			{albums.slice(0, 6).map((album) => (
				<GridCol span={{ base: 12, xs: 6, lg: 4, xl: 2 }} key={album.id}>
					<AlbumCard key={album.id} album={album} />
				</GridCol>
			))}
		</Grid>
	);
}

