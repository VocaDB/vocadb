
module vdb.dataContracts {
	
	export interface AlbumForUserForApiContract {

		album: AlbumContract;

		mediaType: string;

		purchaseStatus: string;

		rating: number;

		user?: user.UserApiContract;

	}

}