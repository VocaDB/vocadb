import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { Group, Text } from '@mantine/core';
import CustomImage from '../Image/Image';
import useSWR from 'swr';
import { apiGet } from '@/Helpers/FetchApiHelper';
import { ArtistType } from '@/types/Models/Artists/ArtistType';

interface ArtistToolTipContentProps {
	artist: ArtistApiContract;
}

export default function ArtistToolTipContent({ artist }: ArtistToolTipContentProps) {
	const { data, isLoading } = useSWR(
		`/api/artists/${artist.id}?fields=AdditionalNames,MainPicture`,
		apiGet<ArtistApiContract>
	);

	const isHuman = (type: ArtistType | undefined): boolean => {
		if (!type) {
			return false;
		}

		return [
			ArtistType.Producer,
			ArtistType.CoverArtist,
			ArtistType.Illustrator,
			ArtistType.Animator,
			ArtistType.Lyricist,
			ArtistType.Circle,
			ArtistType.Label,
		].includes(type);
	};

	return (
		<Group>
			{isLoading && <div style={{ width: 70 }} />}
			{isHuman(data?.artistType) && !isLoading ? (
				<div>
					<CustomImage
						width={70}
						height={70}
						src={data?.mainPicture.urlOriginal ?? '/unknown.png'}
						alt=""
					/>
				</div>
			) : (
				<div style={{ width: 70, height: 100, position: 'relative' }}>
					{data !== undefined && (
						<CustomImage
							style={{ objectFit: 'contain' }}
							src={data.mainPicture.urlOriginal ?? '/unknown.png'}
							fill
							alt=""
						/>
					)}
				</div>
			)}
			<div>
				<Text>
					{artist.name}
					<Text span>
						{' ('}
						{artist.artistType}
						{')'}
					</Text>
				</Text>
				<Text size="sm" color="dimmed">
					{artist.additionalNames
						.split(', ')
						.filter((_name, index) => index < 3)
						.join(', ')}
				</Text>
			</div>
		</Group>
	);
}

