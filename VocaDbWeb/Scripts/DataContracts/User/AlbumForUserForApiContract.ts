import AlbumContract from '../Album/AlbumContract';
import UserApiContract from './UserApiContract';

export default interface AlbumForUserForApiContract {
  album: AlbumContract;

  mediaType: string;

  purchaseStatus: string;

  rating: number;

  user?: UserApiContract;
}
