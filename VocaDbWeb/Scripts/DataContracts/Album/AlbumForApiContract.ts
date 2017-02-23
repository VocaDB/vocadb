module vdb.dataContracts {

	export interface AlbumForApiContract extends AlbumContract {

		artists?: ArtistForAlbumContract[];		

	}

} 