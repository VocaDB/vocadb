import AlbumContract from './AlbumContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';

//module vdb.dataContracts {

	export interface AlbumForApiContract extends AlbumContract {

		artists?: ArtistForAlbumContract[];		

	}

//} 