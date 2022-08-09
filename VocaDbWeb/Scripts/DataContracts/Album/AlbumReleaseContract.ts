import { OptionalDateTimeContract } from '@/DataContracts/OptionalDateTimeContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';

export interface AlbumReleaseContract {
	catNum: string;

	releaseDate?: OptionalDateTimeContract;

	releaseEvent?: ReleaseEventContract;
}
