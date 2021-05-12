import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EntryPictureFileContract from '../EntryPictureFileContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongInAlbumEditContract from '../Song/SongInAlbumEditContract';
import WebLinkContract from '../WebLinkContract';
import AlbumDiscPropertiesContract from './AlbumDiscPropertiesContract';
import AlbumReleaseContract from './AlbumReleaseContract';

export default interface AlbumForEditContract {
  artistLinks: ArtistForAlbumContract[];

  coverPictureMime?: string;

  defaultNameLanguage: string;

  description: EnglishTranslatedStringContract;

  discs: AlbumDiscPropertiesContract[];

  discType: string;

  id: number;

  identifiers: string[];

  names: LocalizedStringWithIdContract[];

  originalRelease: AlbumReleaseContract;

  pictures: EntryPictureFileContract[];

  pvs: PVContract[];

  songs: SongInAlbumEditContract[];

  status: string;

  updateNotes?: string;

  webLinks: WebLinkContract[];
}
