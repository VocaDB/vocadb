import ArtistForEventContract from './ArtistForEventContract';
import EntryThumbContract from '../EntryThumbContract';
import EventSeriesContract from './EventSeriesContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongListBaseContract from '../SongListBaseContract';
import VenueForApiContract from '../Venue/VenueForApiContract';
import WebLinkContract from '../WebLinkContract';

// Matches ReleaseEventForApiContract
export default interface ReleaseEventContract {
  artists: ArtistForEventContract[];

  category: string;

  date?: string;

  defaultNameLanguage: string;

  endDate?: string;

  id: number;

  mainPicture?: EntryThumbContract;

  name: string;

  names?: LocalizedStringWithIdContract[];

  pvs?: PVContract[];

  series?: EventSeriesContract;

  songList?: SongListBaseContract;

  venue?: VenueForApiContract;

  webLinks: WebLinkContract[];
}
