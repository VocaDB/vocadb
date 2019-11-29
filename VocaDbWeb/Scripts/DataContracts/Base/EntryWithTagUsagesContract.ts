
//module vdb.dataContracts {
	
	export interface EntryWithTagUsagesContract {
		
		id: number;

		name: string;

		tags?: tags.TagUsageForApiContract[];

	}

//}