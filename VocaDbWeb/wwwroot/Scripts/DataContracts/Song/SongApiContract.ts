import PVService from '@Models/PVs/PVService';

import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import LocalizedStringContract from '../Globalization/LocalizedStringContract';
import SongContract from './SongContract';

export default interface SongApiContract
  extends SongContract,
    EntryWithTagUsagesContract {
  artists?: ArtistForAlbumContract[];

  names?: LocalizedStringContract[];

  // Not returned from the API, but can be used to cache the list of PV services client side
  pvServicesArray?: PVService[];

  urlFriendlyName?: string;
}
