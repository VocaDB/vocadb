import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { CommonEntryContract } from '@/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { ArtistType } from '@/Models/Artists/ArtistType';

export interface ArtistApiContract
	extends CommonEntryContract,
		EntryWithTagUsagesContract {
	additionalNames: string;
	artistType: ArtistType;
	mainPicture: EntryThumbContract;
	version: number;
}
