import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import LocalizedStringContract from '../Globalization/LocalizedStringContract';
import PVService from '../../Models/PVs/PVService';
import SongContract from './SongContract';

//module vdb.dataContracts {
	
	export default interface SongApiContract extends SongContract, EntryWithTagUsagesContract {

		artists?: ArtistForAlbumContract[];

		names?: LocalizedStringContract[];

		// Not returned from the API, but can be used to cache the list of PV services client side
		pvServicesArray?: PVService[];

		urlFriendlyName?: string;

	}

//}