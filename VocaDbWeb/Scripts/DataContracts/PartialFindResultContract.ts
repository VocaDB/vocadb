
module vdb.dataContracts {
	
	export interface PartialFindResultContract<T> {

		items: T[];

		totalCount: number;

	}

}