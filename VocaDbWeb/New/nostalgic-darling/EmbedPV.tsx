import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { YouTubePlayer } from './players/YouTubePlayer';
import { NiconicoPlayer } from './players/NiconicoPlayer';
import { BilibiliPlayer } from './players/BilibiliPlayer';

interface EmbedPVProps {
	songId: number;
	pv: PVContract;
}

export default function EmbedPV({ pv, songId }: EmbedPVProps) {
	switch (pv.service) {
		case PVService.Youtube:
			return <YouTubePlayer pv={pv} />;
		case PVService.NicoNicoDouga:
			return <NiconicoPlayer pv={pv} />;
		case PVService.Bilibili:
			return <BilibiliPlayer pv={pv} />;
	}

	return <p>Fallback</p>;
}

