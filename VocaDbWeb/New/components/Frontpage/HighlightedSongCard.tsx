import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Button, Card, Group, Text } from '@mantine/core';
import Image from 'next/image';
import Link from 'next/link';

interface HighlightedSongCardProps {
	song: SongWithPVAndVoteContract;
	priority?: boolean;
}

export function HighlightedSongCard({ song, priority }: HighlightedSongCardProps) {
	const pictureSrc = song.mainPicture?.urlOriginal;

	return (
		<Card radius="md" maw={'100vw'}>
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

			<Text mt="md" weight={500}>
				{song.name}
			</Text>
			<Text size="sm" color="dimmed">
				{song.artistString}
			</Text>

			<Group mt="md" position="apart">
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
		</Card>
	);
}

