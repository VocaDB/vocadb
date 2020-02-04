
//module vdb.dataContracts.versioning {
	
	// C# class: ArchivedObjectVersionForApiContract
	export default interface ArchivedVersionContract {
		
		changedFields: string[];

		id: number;

		notes: string;

		version: number;

	}

//} 