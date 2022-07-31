import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';

export interface SongInAlbumRefContract extends ObjectRefContract {
	discNumber: number;
	trackNumber: number;
}
