import AlbumContract from './AlbumContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';

export default interface AlbumForApiContract extends AlbumContract {
  artists?: ArtistForAlbumContract[];
}
