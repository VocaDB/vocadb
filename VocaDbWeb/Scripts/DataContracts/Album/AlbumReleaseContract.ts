
module vdb.dataContracts.albums {
	
	export interface AlbumReleaseContract {

		catNum: string;

		releaseDate: OptionalDateTimeContract;

		releaseEvent?: ReleaseEventContract;

	}

}