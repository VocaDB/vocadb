import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Button, Card, Group, Stack, Text } from '@mantine/core';
import Image from 'next/image';
import Link from 'next/link';

interface HighlightedSongCardProps {
	song: SongWithPVAndVoteContract;
	priority?: boolean;
}

// TODO: Move styles to separate file
export function HighlightedSongCard({ song, priority }: HighlightedSongCardProps) {
	const pictureSrc = song.mainPicture?.urlOriginal;

	return (
		<Card
			style={{ height: '100%', display: 'flex', flexDirection: 'column' }}
			radius="md"
			maw={'100vw'}
		>
			<Card.Section>
				<Image
					src={pictureSrc ?? ''}
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
					<Text>{`something something`}</Text>
					<Button
						component={Link}
						href={`/S/${song.id}`}
						variant="light"
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

