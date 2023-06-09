import { SongInListContract } from '@/types/DataContracts/Song/SongInListContract';

export interface SongInListEditContract extends SongInListContract {
	songInListId: number;
}
