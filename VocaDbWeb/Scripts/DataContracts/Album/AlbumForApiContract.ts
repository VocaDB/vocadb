import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';

export interface AlbumForApiContract extends AlbumContract {
	artists?: ArtistForAlbumContract[];
}
