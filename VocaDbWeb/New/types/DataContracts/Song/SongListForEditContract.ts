import { SongInListEditContract } from '@/types/DataContracts/Song/SongInListEditContract';
import { SongListContract } from '@/types/DataContracts/Song/SongListContract';

export interface SongListForEditContract extends SongListContract {
	songLinks: SongInListEditContract[];
	updateNotes?: string;
}
