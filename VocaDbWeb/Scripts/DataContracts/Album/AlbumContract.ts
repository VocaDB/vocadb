import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { CommonEntryContract } from '@/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { OptionalDateTimeContract } from '@/DataContracts/OptionalDateTimeContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { EntryStatus } from '@/Models/EntryStatus';

export interface AlbumContract
	extends CommonEntryContract,
		EntryWithTagUsagesContract {
	additionalNames: string;
	artistString: string;
	discType: AlbumType;
	mainPicture: EntryThumbContract;
	ratingAverage: number;
	ratingCount: number;
	releaseDate: OptionalDateTimeContract;
	releaseEvent?: ReleaseEventContract;
	releaseEvents: ReleaseEventContract[];
	status: EntryStatus;
	version: number;
}
