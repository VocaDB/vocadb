import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { OptionalDateTimeContract } from '@/types/DataContracts/OptionalDateTimeContract';

export interface ArchivedAlbumReleaseContract {
	catNum?: string;
	releaseDate?: OptionalDateTimeContract;
	releaseEvent?: ObjectRefContract;
}
