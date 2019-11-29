import AlbumContract from './AlbumContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';

//module vdb.dataContracts {

	export default interface AlbumForApiContract extends AlbumContract {

		artists?: ArtistForAlbumContract[];		

	}

//} 