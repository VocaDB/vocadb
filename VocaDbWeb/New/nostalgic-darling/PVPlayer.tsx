import EmbedPV from './EmbedPV';
import { usePlayerStore } from './stores/usePlayerStore';

export default function PVPlayer() {
	const [playerBounds, pv] = usePlayerStore((set) => [set.playerBounds, set.pv]);

	if (pv === undefined) {
		return <></>;
	}

	return (
		<div
			style={
				playerBounds === undefined
					? {
							position: 'relative',
							zIndex: -10,
					  }
					: {
							position: 'fixed',
							left: playerBounds.x,
							top: playerBounds.y, // We have to subtract the header height
							width: playerBounds.width,
							height: playerBounds.height,
					  }
			}
		>
			<EmbedPV pv={pv} />
		</div>
	);
}

