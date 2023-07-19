import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { OptionalDateTimeContract } from '@/DataContracts/OptionalDateTimeContract';

export interface ArchivedAlbumReleaseContract {
	catNum?: string;
	releaseDate?: OptionalDateTimeContract;
	releaseEvent?: ObjectRefContract;
	releaseEvents?: ObjectRefContract[];
}
