
import AlbumContract from '../Album/AlbumContract';
import UserApiContract from './UserApiContract';

//module vdb.dataContracts {
	
	export interface AlbumForUserForApiContract {

		album: AlbumContract;

		mediaType: string;

		purchaseStatus: string;

		rating: number;

		user?: UserApiContract;

	}

//}