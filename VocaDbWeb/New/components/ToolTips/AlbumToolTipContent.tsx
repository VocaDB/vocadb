import { formatComponentDate } from '@/Helpers/DateTimeHelper';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Group, Rating, Text } from '@mantine/core';

export interface AlbumToolTipProps {
	entry: 'album';
	album: AlbumForApiContract;
}

export default function AlbumToolTipContent({ album }: AlbumToolTipProps) {
	const releaseDate = formatComponentDate(
		album.releaseDate.year,
		album.releaseDate.month,
		album.releaseDate.day
	);
	return (
		<>
			<Text>{album.name}</Text>
			<Text size="sm" color="dimmed">
				{album.artistString}
			</Text>
			{!album.releaseDate.isEmpty && (
				<Text size="sm" mt="md">
					{'Released '} {releaseDate}
				</Text>
			)}
			{album.ratingCount > 0 && (
				<Group mt="xs">
					<Rating readOnly value={album.ratingAverage} />({album.ratingCount} Ratings)
				</Group>
			)}
		</>
	);
}

