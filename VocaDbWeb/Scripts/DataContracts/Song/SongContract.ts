import CommonEntryContract from '@/DataContracts/CommonEntryContract';
import EntryThumbContract from '@/DataContracts/EntryThumbContract';
import SongType from '@/Models/Songs/SongType';

export default interface SongContract extends CommonEntryContract {
	additionalNames: string;

	artistString: string;

	favoritedTimes?: number;

	lengthSeconds: number;

	mainPicture?: EntryThumbContract;

	// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
	publishDate?: string;

	pvServices: string;

	ratingScore: number;

	songType: SongType;

	status: string;

	thumbUrl?: string;
}
