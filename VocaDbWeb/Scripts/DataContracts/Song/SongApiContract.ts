import ArtistForAlbumContract from '@/DataContracts/ArtistForAlbumContract';
import EntryWithTagUsagesContract from '@/DataContracts/Base/EntryWithTagUsagesContract';
import LocalizedStringContract from '@/DataContracts/Globalization/LocalizedStringContract';
import SongContract from '@/DataContracts/Song/SongContract';
import PVService from '@/Models/PVs/PVService';

export default interface SongApiContract
	extends SongContract,
		EntryWithTagUsagesContract {
	artists?: ArtistForAlbumContract[];

	defaultName?: string;

	names?: LocalizedStringContract[];

	// Not returned from the API, but can be used to cache the list of PV services client side
	pvServicesArray?: PVService[];

	urlFriendlyName?: string;

	version?: number;
}
