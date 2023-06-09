import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';

export interface AlbumForSongRefContract extends ObjectRefContract {
	discNumber: number;
	trackNumber: number;
}
