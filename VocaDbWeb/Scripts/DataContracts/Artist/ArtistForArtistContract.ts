import ArtistContract from './ArtistContract';

//module vdb.dataContracts.artists {
	
	export default interface ArtistForArtistContract {

		parent: ArtistContract;

		// Link ID
		id?: number;

		linkType?: string;

	}

//}