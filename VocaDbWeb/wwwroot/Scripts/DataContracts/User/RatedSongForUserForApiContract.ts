import SongApiContract from '../Song/SongApiContract';
import UserApiContract from './UserApiContract';

export default interface RatedSongForUserForApiContract {
  rating: string;

  song?: SongApiContract;

  user?: UserApiContract;
}
