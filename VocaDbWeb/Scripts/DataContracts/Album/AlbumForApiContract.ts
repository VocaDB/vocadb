import ArtistForAlbumContract from '../ArtistForAlbumContract';
import AlbumContract from './AlbumContract';

export default interface AlbumForApiContract extends AlbumContract {
  artists?: ArtistForAlbumContract[];
}
