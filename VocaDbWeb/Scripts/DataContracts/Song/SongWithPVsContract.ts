import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';

export interface SongWithPVsContract extends SongApiContract {
	entryType: string /* TODO: enum */;
	pvs: PVContract[];
}
