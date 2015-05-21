
module vdb.dataContracts {
	
	// Base data contract for entries from the API.
	// Corresponds to C# datacontract EntryForApiContract.
	export interface EntryContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		artistType?: string;

		discType?: string;

		entryType: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

		songType?: string;

	}

}