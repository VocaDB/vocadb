import { CommonEntryContract } from '@/types/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { SongType } from '@/types/Models/Songs/SongType';

export interface SongContract extends CommonEntryContract {
	additionalNames: string;
	artistString: string;
	favoritedTimes?: number;
	lengthSeconds: number;
	mainPicture?: EntryThumbContract;
	// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
	publishDate?: string;
	pvs?: PVContract[];
	pvServices: string;
	ratingScore: number;
	songType: SongType;
	status: EntryStatus;
	thumbUrl?: string;
	version: number;
}
