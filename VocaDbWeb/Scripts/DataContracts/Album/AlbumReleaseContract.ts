import OptionalDateTimeContract from '../OptionalDateTimeContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';

export default interface AlbumReleaseContract {
	catNum: string;

	releaseDate: OptionalDateTimeContract;

	releaseEvent?: ReleaseEventContract;
}
