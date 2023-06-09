import { EntryWithTagUsagesContract } from '@/types/DataContracts/Base/EntryWithTagUsagesContract';
import { CommonEntryContract } from '@/types/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';

export interface ArtistApiContract extends CommonEntryContract, EntryWithTagUsagesContract {
	additionalNames: string;
	artistType: ArtistType;
	mainPicture: EntryThumbContract;
	version: number;
}
