import SongType from '@Models/Songs/SongType';

import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';

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
