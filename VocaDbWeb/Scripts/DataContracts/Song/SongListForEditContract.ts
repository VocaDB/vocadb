import { SongInListEditContract } from '@/DataContracts/Song/SongInListEditContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';

export interface SongListForEditContract extends SongListContract {
	songLinks: SongInListEditContract[];
	updateNotes?: string;
}
