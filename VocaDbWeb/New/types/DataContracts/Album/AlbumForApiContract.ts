import { AlbumContract } from '@/types/DataContracts/Album/AlbumContract';
import { ArtistForAlbumContract } from '@/types/DataContracts/ArtistForAlbumContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongInAlbumContract } from '@/types/DataContracts/Song/SongInAlbumContract';

export interface AlbumForApiContract extends AlbumContract {
	artists?: ArtistForAlbumContract[];
	pvs?: PVContract[];
	tracks?: SongInAlbumContract[];
}
