import EntryThumbContract from '../EntryThumbContract';
import UserBaseContract from './UserBaseContract';

	export default interface UserApiContract extends UserBaseContract {
		
		mainPicture?: EntryThumbContract;

	}