
module vdb.dataContracts {

	export interface ArtistApiContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		mainPicture: EntryThumbContract;

	}

} 