
//module vdb.dataContracts.versioning {
	
	// C# class: ArchivedObjectVersionForApiContract
	export interface ArchivedVersionContract {
		
		changedFields: string[];

		id: number;

		notes: string;

		version: number;

	}

//} 