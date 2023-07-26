import { Group, Paper, Slider, Space, Title, useMantineTheme } from '@mantine/core';
import { motion } from 'framer-motion';
import Sheet from 'react-modal-sheet';
import CustomImage from '../Image/Image';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { getBestThumbUrl } from '../Frontpage/HighlightedSongCard';
import { IconPlayerPlay, IconPlayerTrackPrev } from '@tabler/icons';
import { IconPlayerPlayFilled, IconPlayerTrackNext } from '@tabler/icons-react';

interface MobilePlayerProps {
	expanded: boolean;
	onClose(): void;
}

export default function MobilePlayer({ expanded, onClose }: MobilePlayerProps) {
	const theme = useMantineTheme();
	const [song] = usePlayerStore((state) => [state.song]);
	const bestUrl = getBestThumbUrl(song?.pvs ?? []);
	return (
		<Sheet isOpen={expanded} onClose={onClose} snapPoints={[600, 0]}>
			<Sheet.Container style={{ backgroundColor: theme.colors.dark[6] }}>
				<Sheet.Header />
				<Sheet.Content>
					<div
						style={{ maxWidth: 'min-content', marginRight: 'auto', marginLeft: 'auto' }}
					>
						<Space h="md" />
						<CustomImage
							height={300}
							width={300}
							src={
								bestUrl === undefined
									? '/unknown.webp'
									: '/api/pvs/thumbnail?pvUrl=' + bestUrl
							}
							mode="crop"
							alt="Song thumbnail"
						/>
						<Title mt="xl" order={2}>
							{song?.name}
						</Title>
						<Title size="sm" color="dimmed" order={3}>
							{song?.artistString}
						</Title>
						<Slider
							mt="xl"
							defaultValue={20}
							marks={[
								{ value: 0, label: '0:00' },
								{ value: 180, label: '3:00' },
							]}
						/>
						<Group mt="md" position="center">
							<IconPlayerTrackPrev size="2rem" />
							<IconPlayerPlay size="3rem" />
							<IconPlayerTrackNext size="2rem" />
						</Group>
					</div>
				</Sheet.Content>
			</Sheet.Container>
			<Sheet.Backdrop />
		</Sheet>
	);
}

