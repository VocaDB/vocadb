import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Button, Card, Group, Stack, Text } from '@mantine/core';
import Image from 'next/image';
import Link from 'next/link';

interface HighlightedSongCardProps {
	song: SongWithPVAndVoteContract;
	priority?: boolean;
}

const PREFERRED_SERVICES = ['Youtube', 'NicoNicoDouga', 'Bilibili', 'Vimeo'];

const getBestThumbUrl = (pvs: PVContract[]): string | undefined => {
	return pvs
		.filter((pv) => !pv.disabled && pv.url !== undefined)
		.reduce((currPV: PVContract | undefined, nextPV) => {
			const currPos = PREFERRED_SERVICES.indexOf(currPV?.service ?? '');
			const nextPos = PREFERRED_SERVICES.indexOf(nextPV.service ?? '');
			if (
				currPV === undefined ||
				(PREFERRED_SERVICES.includes(nextPV.service) && nextPos > currPos)
			) {
				return nextPV;
			}
			return currPV;
		}, undefined)?.url;
};

// TODO: Move styles to separate file
export function HighlightedSongCard({ song, priority }: HighlightedSongCardProps) {
	const bestThumbUrl = getBestThumbUrl(song.pvs);

	if (!bestThumbUrl) {
		return <></>;
	}

	return (
		<Card
			style={{ height: '100%', display: 'flex', flexDirection: 'column' }}
			radius="md"
			withBorder
			shadow="sm"
		>
			<Card.Section>
				{/* TODO: Move the url creation code into a separate function */}
				<Image
					src={bestThumbUrl}
					loader={(props) =>
						`${process.env.NEXT_PUBLIC_API_URL}/api/pvs/thumbnail?pvUrl=${props.src}`
					}
					blurDataURL={song.mainPicture?.urlSmallThumb}
					height={240}
					width={360}
					style={{ width: '100%', objectFit: 'cover' }}
					alt={`${song.name} thumbnail`}
					priority={priority}
				/>
			</Card.Section>
			<Stack mt="md" style={{ justifyContent: 'space-between', flex: 2 }}>
				<div>
					<Text weight={500}>{song.name}</Text>
					<Text size="sm" color="dimmed">
						{song.artistString}
					</Text>
				</div>

				<Group position="apart">
					<Text>{`Score: ${song.ratingScore}`}</Text>
					<Button
						component={Link}
						href={`/S/${song.id}`}
						variant="outline"
						radius="md"
						title="Song Info"
					>
						Song Info
					</Button>
				</Group>
			</Stack>
		</Card>
	);
}

