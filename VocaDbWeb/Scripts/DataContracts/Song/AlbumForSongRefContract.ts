import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';

export interface AlbumForSongRefContract extends ObjectRefContract {
	discNumber: number;
	trackNumber: number;
}
