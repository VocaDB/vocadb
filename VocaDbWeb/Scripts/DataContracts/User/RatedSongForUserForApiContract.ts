
import SongApiContract from '../Song/SongApiContract';
import UserApiContract from './UserApiContract';

//namespace vdb.dataContracts {
	
	export default interface RatedSongForUserForApiContract {

		rating: string;

		song?: SongApiContract;

		user?: UserApiContract;

	}

//}