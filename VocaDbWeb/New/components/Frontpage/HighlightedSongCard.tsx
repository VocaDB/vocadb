import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Button, Card, Group, Stack, Text } from '@mantine/core';
import Image from 'next/image';
import Link from 'next/link';
import useStyles from './HighlightedSongCard.styles';
import { IconHeart, IconThumbUp } from '@tabler/icons-react';

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

export function HighlightedSongCard({ song, priority }: HighlightedSongCardProps) {
	const styles = useStyles();

	const bestThumbUrl = getBestThumbUrl(song.pvs);

	if (!bestThumbUrl) {
		return <></>;
	}

	return (
		<Card className={styles.classes.card} radius="md" withBorder shadow="sm">
			<Card.Section>
				{/* TODO: Move the url creation code into a separate function */}
				<Image
					src={`${process.env.NEXT_PUBLIC_API_URL}/api/pvs/thumbnail?pvUrl=${bestThumbUrl}`}
					blurDataURL={song.mainPicture?.urlSmallThumb}
					height={240}
					width={360}
					className={styles.classes.image}
					alt={`${song.name} thumbnail`}
					priority={priority}
				/>
			</Card.Section>
			<Stack mt="md" className={styles.classes.contentStack}>
				<div>
					<Text weight={500}>{song.name}</Text>
					<Text size="sm" color="dimmed">
						{song.artistString}
					</Text>
				</div>

				<Group position="apart">
					<Group spacing="xs">
						<>
							<IconThumbUp stroke={'1.5'} size="1.1rem" />

							<Text style={{ stroke: '1.5' }}>
								{(song.ratingScore - 3 * (song.favoritedTimes ?? 0)) / 2}
							</Text>
						</>
						<>
							<IconHeart stroke={'1.5'} size="1.1rem" />
							<Text style={{ stroke: '1.5' }}>{song.favoritedTimes}</Text>
						</>
					</Group>
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

