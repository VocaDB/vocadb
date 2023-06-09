import { ArtistForAlbumContract } from '@/types/DataContracts/ArtistForAlbumContract';
import { EntryWithTagUsagesContract } from '@/types/DataContracts/Base/EntryWithTagUsagesContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { SongContract } from '@/types/DataContracts/Song/SongContract';
import { PVService } from '@/types/Models/PVs/PVService';

export interface SongApiContract extends SongContract, EntryWithTagUsagesContract {
	artists?: ArtistForAlbumContract[];
	defaultName?: string;
	names?: LocalizedStringContract[];
	// Not returned from the API, but can be used to cache the list of PV services client side
	pvServicesArray?: PVService[];
	urlFriendlyName?: string;
}
