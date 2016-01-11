
module vdb.dataContracts {
	
	export interface TagBaseContract {
		// Additional names list - optional field
		additionalNames?: string; 
		id: number;
		name: string;
		urlSlug?: string;
	}

}