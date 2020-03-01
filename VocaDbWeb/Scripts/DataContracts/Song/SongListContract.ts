import SongListBaseContract from '../SongListBaseContract';
import UserBaseContract from '../User/UserBaseContract';

//module vdb.dataContracts {

    export default interface SongListContract extends SongListBaseContract {
        
        author: UserBaseContract;

        description: string;

		eventDate?: string;

        featuredCategory: string;
    
		status: string;

    }

//}