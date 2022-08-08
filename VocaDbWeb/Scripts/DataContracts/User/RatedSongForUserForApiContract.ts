import { SongVoteRating } from '@/Stores/Search/SongSearchStore';

import SongApiContract from '../Song/SongApiContract';
import UserApiContract from './UserApiContract';

export default interface RatedSongForUserForApiContract {
	rating: SongVoteRating;

	song?: SongApiContract;

	user?: UserApiContract;
}
