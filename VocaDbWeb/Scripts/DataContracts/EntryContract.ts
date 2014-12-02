
module vdb.dataContracts {
	
	// Base data contract for entries from the API.
	// Corresponds to C# datacontract EntryForApiContract.
	export interface EntryContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		entryType: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

	}

}