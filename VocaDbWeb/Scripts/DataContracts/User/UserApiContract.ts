import EntryThumbContract from '../EntryThumbContract';
import UserBaseContract from './UserBaseContract';

//module vdb.dataContracts.user {
	
	export default interface UserApiContract extends UserBaseContract {
		
		mainPicture?: EntryThumbContract;

	}

//}