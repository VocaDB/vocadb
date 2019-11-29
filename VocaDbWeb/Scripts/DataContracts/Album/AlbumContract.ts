
//module vdb.dataContracts {

	export interface AlbumContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		artistString: string;

		discType: string;

		mainPicture: EntryThumbContract;

		ratingAverage: number;

		ratingCount: number;

		releaseDate: OptionalDateTimeContract;

	}

//} 