
module vdb.dataContracts.artists {
	
	export interface ArtistForArtistContract {

		parent: ArtistContract;

		// Link ID
		id?: number;

		linkType?: string;

	}

}