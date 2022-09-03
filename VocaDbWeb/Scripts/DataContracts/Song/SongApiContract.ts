import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { PVService } from '@/Models/PVs/PVService';

export interface SongApiContract
	extends SongContract,
		EntryWithTagUsagesContract {
	artists?: ArtistForAlbumContract[];

	defaultName?: string;

	names?: LocalizedStringContract[];

	pvs?: PVContract[];

	// Not returned from the API, but can be used to cache the list of PV services client side
	pvServicesArray?: PVService[];

	urlFriendlyName?: string;

	version?: number;
}
