import OptionalDateTimeContract from '@/DataContracts/OptionalDateTimeContract';
import ReleaseEventContract from '@/DataContracts/ReleaseEvents/ReleaseEventContract';

export default interface AlbumReleaseContract {
	catNum: string;

	releaseDate?: OptionalDateTimeContract;

	releaseEvent?: ReleaseEventContract;
}
