import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongInAlbumContract } from '@/DataContracts/Song/SongInAlbumContract';

export interface AlbumForApiContract extends AlbumContract {
	artists?: ArtistForAlbumContract[];
	pvs?: PVContract[];
	tracks?: SongInAlbumContract[];
}
