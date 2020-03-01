import ArtistForEventContract from './ArtistForEventContract';
import EntryThumbContract from '../EntryThumbContract';
import EventSeriesContract from './EventSeriesContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongListBaseContract from '../SongListBaseContract';
import WebLinkContract from '../WebLinkContract';

//namespace vdb.dataContracts {

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

		webLinks: WebLinkContract[];

	}

//}