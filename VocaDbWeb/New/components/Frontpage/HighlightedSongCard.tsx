import { SongWithPVAndVoteContract } from '@/types/DataContracts/Song/SongWithPVAndVoteContract';
import { Button, Card, CardSection, Group, Space, Text } from '@mantine/core';
import Link from 'next/link';
import { IconHeart, IconThumbUp } from '@tabler/icons-react';
import CustomImage from '../Image/Image';
import { getBestThumbImageUrl } from '@/Helpers/getBestThumbUrl';
import styles from './HighlightedSongCard.module.css';

interface HighlightedSongCardProps {
	song: SongWithPVAndVoteContract;
	priority?: boolean;
}

export function HighlightedSongCard({ song, priority }: HighlightedSongCardProps) {
	const bestThumbImageUrl = getBestThumbImageUrl(song.pvs);

	return (
		<Card className={styles.card} radius="md" withBorder shadow="sm">
			<CardSection>
				<CustomImage
					src={bestThumbImageUrl}
					height={240}
					width={360}
					className={styles.image}
					alt={`${song.name} thumbnail`}
					priority={priority}
				/>
			</CardSection>
			<div className={styles.textSectionWrapper}>
				<div>
					<Text mt="xs" fw={500}>
						{song.name}
					</Text>
					<Text size="sm" c="dimmed">
						{song.artistString}
					</Text>
				</div>

				<Space h="md" />

				<Group justify="space-between">
					<Group gap="xs">
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
			</div>
		</Card>
	);
}

