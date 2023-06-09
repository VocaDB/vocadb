import { OptionalDateTimeContract } from '@/types/DataContracts/OptionalDateTimeContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';

export interface AlbumReleaseContract {
	catNum: string;

	releaseDate?: OptionalDateTimeContract;

	releaseEvent?: ReleaseEventContract;
}
