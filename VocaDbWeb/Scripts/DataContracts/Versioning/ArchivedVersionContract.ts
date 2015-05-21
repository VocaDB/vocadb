
module vdb.dataContracts.versioning {
	
	export interface ArchivedVersionContract {
		
		changedFields: string;

		id: number;

		notes: string;

		version: number;

	}

} 