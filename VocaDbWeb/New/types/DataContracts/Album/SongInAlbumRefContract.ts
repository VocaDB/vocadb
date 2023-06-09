import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';

export interface SongInAlbumRefContract extends ObjectRefContract {
	discNumber: number;
	trackNumber: number;
}
