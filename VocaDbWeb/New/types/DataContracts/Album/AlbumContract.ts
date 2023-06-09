import { EntryWithTagUsagesContract } from '@/types/DataContracts/Base/EntryWithTagUsagesContract';
import { CommonEntryContract } from '@/types/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { OptionalDateTimeContract } from '@/types/DataContracts/OptionalDateTimeContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';
import { EntryStatus } from '@/types/Models/EntryStatus';

export interface AlbumContract extends CommonEntryContract, EntryWithTagUsagesContract {
	additionalNames: string;
	artistString: string;
	discType: AlbumType;
	mainPicture: EntryThumbContract;
	ratingAverage: number;
	ratingCount: number;
	releaseDate: OptionalDateTimeContract;
	releaseEvent?: ReleaseEventContract;
	status: EntryStatus;
	version: number;
}
