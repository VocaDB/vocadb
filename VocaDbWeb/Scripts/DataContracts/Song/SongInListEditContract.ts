import { SongInListContract } from '@/DataContracts/Song/SongInListContract';

export interface SongInListEditContract extends SongInListContract {
	songInListId: number;
}
