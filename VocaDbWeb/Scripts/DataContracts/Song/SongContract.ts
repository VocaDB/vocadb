import CommonEntryContract from '../CommonEntryContract';

export default interface SongContract extends CommonEntryContract {
	additionalNames: string;

	artistString: string;

	favoritedTimes?: number;

	lengthSeconds: number;

	// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
	publishDate?: string;

	pvServices: string;

	ratingScore: number;

	songType: string;

	status: string;

	thumbUrl?: string;
}
