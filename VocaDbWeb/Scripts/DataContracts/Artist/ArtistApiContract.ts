
//module vdb.dataContracts {

	export interface ArtistApiContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		artistType: string;

		mainPicture: EntryThumbContract;

	}

//} 