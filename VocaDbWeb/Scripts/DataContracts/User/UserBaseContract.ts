import IEntryWithIdAndName from '../../Models/IEntryWithIdAndName';

//module vdb.dataContracts {

	export default interface UserBaseContract extends IEntryWithIdAndName {
        
        id: number;

        name?: string;
    
    }

//}