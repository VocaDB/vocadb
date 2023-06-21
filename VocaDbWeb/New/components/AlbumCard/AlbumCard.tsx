import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Grid, Title } from '@mantine/core';
import Image from 'next/image';
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
							width: '250px',
							height: '250px',
							background: 'linear-gradient(to top, rgba(0, 0, 0, 0.70), transparent)',
						}}
					>
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
		<Grid style={{ maxWidth: '516px' }}>
			{albums.slice(0, 4).map((album) => (
				<Grid.Col xl={6} key={album.id}>
					<AlbumCard key={album.id} album={album} />
				</Grid.Col>
			))}
		</Grid>
	);
}

