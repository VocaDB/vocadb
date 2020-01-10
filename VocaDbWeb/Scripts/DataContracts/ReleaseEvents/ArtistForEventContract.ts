
import ArtistContract from '../Artist/ArtistContract';

//namespace vdb.dataContracts.events {

	export default interface ArtistForEventContract {

		artist?: ArtistContract;

		id?: number;

		name?: string;

		roles: string;

	}

//}