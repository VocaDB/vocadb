import { Group, Paper, Slider, Space, Title, useMantineTheme } from '@mantine/core';
import Sheet from 'react-modal-sheet';
import CustomImage from '../Image/Image';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { IconPlayerPause, IconPlayerPlay, IconPlayerTrackPrev } from '@tabler/icons';
import { IconPlayerTrackNext } from '@tabler/icons-react';
import { useInterval } from '@mantine/hooks';
import { useEffect, useState } from 'react';
import { formatNumberToTime } from '@/Helpers/DateTimeHelper';
import { getBestThumbUrl } from '@/Helpers/getBestThumbUrl';

const MobilePlayerSheet = () => {
	const [song, active, playerApi] = usePlayerStore((state) => [
		state.song,
		state.active,
		state.playerApi,
	]);
	const bestUrl = getBestThumbUrl(song?.pvs ?? []);

	const [duration, setDuration] = useState(0);
	const [progress, setProgress] = useState(0);

	const interval = useInterval(() => {
		if (playerApi === undefined) return;
		setProgress(playerApi.getCurrentTime() / playerApi.getDuration());
		setDuration(playerApi.getDuration());
	}, 500);

	useEffect(() => {
		interval.start();
		return interval.stop;
	}, [playerApi]);

	if (song === undefined) {
		return <></>;
	}

	return (
		<div style={{ maxWidth: 'min-content', marginRight: 'auto', marginLeft: 'auto' }}>
			<Space h="md" />
			<CustomImage
				height={300}
				width={300}
				style={{ maxHeight: '50vh' }}
				src={
					bestUrl === undefined ? '/unknown.webp' : '/api/pvs/thumbnail?pvUrl=' + bestUrl
				}
				mode="crop"
				alt="Song thumbnail"
			/>
			<Title mt="sm" order={2}>
				{song?.name}
			</Title>
			<Title size="sm" color="dimmed" order={3}>
				{song?.artistString}
			</Title>
			<Slider
				mt="xl"
				value={progress * 100}
				onChange={(newProgress) => {
					if (interval.active) interval.stop();
					setProgress(newProgress / 100);
				}}
				onChangeEnd={(progress) => {
					interval.start();
					if (playerApi === undefined) return;
					playerApi.setCurrentTime((progress / 100) * playerApi.getDuration());
				}}
				label={null}
				showLabelOnHover={false}
				marks={[
					{ value: 0, label: formatNumberToTime(progress * duration) },
					{ value: duration, label: formatNumberToTime(duration) },
				]}
			/>
			<Group mt="md" position="center">
				<IconPlayerTrackPrev size="2rem" />
				{active ? (
					<IconPlayerPause onClick={() => playerApi?.pause()} size="3rem" />
				) : (
					<IconPlayerPlay onClick={() => playerApi?.play()} size="3rem" />
				)}
				<IconPlayerTrackNext size="2rem" />
			</Group>
		</div>
	);
};

interface MobilePlayerProps {
	expanded: boolean;
	onClose(): void;
}

export default function MobilePlayer({ expanded, onClose }: MobilePlayerProps) {
	const theme = useMantineTheme();

	return (
		<Sheet rootId="__next" isOpen={expanded} onClose={onClose} snapPoints={[0.9, 0]}>
			<Sheet.Container
				style={{
					backgroundColor:
						theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
				}}
			>
				<Sheet.Header />
				<Sheet.Content>
					<MobilePlayerSheet />
				</Sheet.Content>
			</Sheet.Container>
			<Sheet.Backdrop />
		</Sheet>
	);
}

