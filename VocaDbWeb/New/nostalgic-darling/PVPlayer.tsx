import { PVService } from '@/types/Models/PVs/PVService';
import EmbedPV from './EmbedPV';
import { usePlayerStore } from './stores/usePlayerStore';

export default function PVPlayer() {
	const [song, playerBounds] = usePlayerStore((set) => [set.song, set.playerBounds]);
	const pv = song?.pvs?.find((pv) => pv.service === PVService.Youtube);
	console.log(pv?.url);

	return (
		<div
			style={
				playerBounds === undefined
					? {
							width: '100%',
							height: '80%',
					  }
					: {
							position: 'absolute',
							left: playerBounds.x,
							top: playerBounds.y - 70, // We have to subtract the header height
							width: playerBounds.width,
							height: playerBounds.height,
					  }
			}
		>
			{pv !== undefined && <EmbedPV pv={pv} />}
		</div>
	);
}

