import { useMantineTheme } from '@mantine/core';
import EmbedPV from './EmbedPV';
import { usePlayerStore } from './stores/usePlayerStore';
import { useMediaQuery } from '@mantine/hooks';

export default function PVPlayer() {
	const theme = useMantineTheme();
	const isMobile = useMediaQuery(`(max-width: ${theme.breakpoints['sm']})`);
	const isIpad = useMediaQuery(`(max-width: ${theme.breakpoints['lg']})`);
	const [playerBounds, pv] = usePlayerStore((set) => [set.playerBounds, set.pv]);

	if (pv === undefined) {
		return <></>;
	}

	// Values from Navbar.tsx
	const subtractLeft = isMobile ? 0 : isIpad ? 220 : 300;
	const subtractTop = isMobile ? 50 : 70;

	return (
		<div
			style={
				playerBounds === undefined
					? {
							position: 'relative',
							zIndex: -10,
					  }
					: {
							position: 'absolute',
							left: playerBounds.x - subtractLeft,
							top: playerBounds.y - subtractTop,
							width: playerBounds.width,
							height: playerBounds.height,
					  }
			}
		>
			<EmbedPV pv={pv} />
		</div>
	);
}

