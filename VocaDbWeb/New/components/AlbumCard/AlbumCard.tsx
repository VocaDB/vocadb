import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, GridCol, UnstyledButton, Text } from '@mantine/core';
import CustomImage from '../Image/Image';
import EntryToolTip from '../ToolTips/EntryToolTip';
import styles from './AlbumCard.module.css';

interface AlbumCardProps {
	album: AlbumForApiContract;
}

export function AlbumCard({ album }: AlbumCardProps) {
	return (
		<EntryToolTip entry="album" album={album}>
			<UnstyledButton className={styles.button}>
				<div style={{ overflow: 'hidden' }}>
					<CustomImage
						src={album.mainPicture?.urlOriginal}
						className={styles.image}
						width={188}
						height={188}
						mode="crop"
						alt="Preview"
					/>
				</div>
				<Text fw={1000} w={180} mt="xs">
					{album.name}
				</Text>
				<Text w={180} fw={500} c="dimmed">
					{album.artistString.split('feat.')[0]}
				</Text>
			</UnstyledButton>
		</EntryToolTip>
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
					<AlbumCard album={album} />
				</GridCol>
			))}
		</Grid>
	);
}

