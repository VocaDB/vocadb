
module vdb.dataContracts {

	export interface UserBaseContract extends vdb.models.IEntryWithIdAndName {
        
        id: number;

        name?: string;
    
    }

}