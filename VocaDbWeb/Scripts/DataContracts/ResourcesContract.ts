
module vdb.dataContracts {
	
	// From ResourcesApiController
	export interface ResourcesContract {

		albumMediaTypeNames?: { [key: string]: string; }

		albumSortRuleNames?: { [key: string]: string; }

		artistSortRuleNames?: { [key: string]: string; }

		discTypeNames?: { [key: string]: string; }

		entryTypeNames?: { [key: string]: string; }

		songSortRuleNames?: { [key: string]: string; }

		songTypeNames?: { [key: string]: string; }

		userGroupNames?: { [key: string]: string; }

	}

}