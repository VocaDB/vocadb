import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { ActionIcon, Group } from '@mantine/core';
import { IconPlayerPause, IconPlayerPlay } from '@tabler/icons-react';

export default function PlayerControls() {
	const [song, active, playerApi] = usePlayerStore((state) => [
		state.song,
		state.active,
		state.playerApi?.current,
	]);

	if (song === undefined) {
		return <></>;
	}

	return (
		<Group position="center">
			{song.name} {' by '} {song.artistString}
			{active ? (
				<ActionIcon onClick={() => playerApi?.pause()}>
					<IconPlayerPause />
				</ActionIcon>
			) : (
				<ActionIcon onClick={() => playerApi?.play()}>
					<IconPlayerPlay />
				</ActionIcon>
			)}
		</Group>
	);
}

