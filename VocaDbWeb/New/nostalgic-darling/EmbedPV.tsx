import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { YouTubePlayer } from './YouTubePlayer';

interface EmbedPVProps {
	songId: number;
	pv: PVContract;
}

export default function EmbedPV({ pv, songId }: EmbedPVProps) {
	switch (pv.service) {
		case PVService.Youtube:
			return <YouTubePlayer songId={songId} videoId={pv.pvId} />;
	}

	return <p>Fallback</p>;
}

