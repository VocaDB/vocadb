import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { YouTubePlayer } from './YouTubePlayer';

interface EmbedPVProps {
	pv: PVContract;
}

export default function EmbedPV({ pv }: EmbedPVProps) {
	switch (pv.service) {
		case PVService.Youtube:
			return <YouTubePlayer videoId={pv.pvId} />;
	}

	return <p>Fallback</p>;
}

