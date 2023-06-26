import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { ActionIcon, Group } from '@mantine/core';
import { IconPlayerPause, IconPlayerPlay } from '@tabler/icons-react';

export default function PlayerControls() {
	const [active, playerApi] = usePlayerStore((state) => [state.active, state.playerApi?.current]);

	return (
		<Group position="center">
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

