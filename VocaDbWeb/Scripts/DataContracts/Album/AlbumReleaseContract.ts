import OptionalDateTimeContract from '../OptionalDateTimeContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';

//module vdb.dataContracts.albums {
	
	export default interface AlbumReleaseContract {

		catNum: string;

		releaseDate: OptionalDateTimeContract;

		releaseEvent?: ReleaseEventContract;

	}

//}