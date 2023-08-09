import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';
// import { SongVoteRating } from '@/types/Stores/Search/SongSearchStore';

export interface RatedSongForUserForApiContract {
	// TODO: Put this back in
	// rating: SongVoteRating;

	song?: SongApiContract;

	user?: UserApiContract;

	date: string;
}
