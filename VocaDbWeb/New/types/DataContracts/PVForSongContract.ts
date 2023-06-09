import { PVContractBase } from './PVs/PVContract';
import { SongApiContract } from './Song/SongApiContract';

// C# class: PVForSongContract
export interface PVForSongContract extends PVContractBase {
	song: SongApiContract;
}
