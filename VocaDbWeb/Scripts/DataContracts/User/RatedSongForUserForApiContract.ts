import SongApiContract from '@/DataContracts/Song/SongApiContract';
import UserApiContract from '@/DataContracts/User/UserApiContract';
import { SongVoteRating } from '@/Stores/Search/SongSearchStore';

export default interface RatedSongForUserForApiContract {
	rating: SongVoteRating;

	song?: SongApiContract;

	user?: UserApiContract;
}
