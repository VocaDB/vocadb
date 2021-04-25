import ArtistContract from './Artist/ArtistContract';

export default interface ArtistForAlbumContract {
  artist: ArtistContract;

  id?: number;

  isCustomName?: boolean;

  isSupport?: boolean;

  name?: string;

  roles: string;
}
