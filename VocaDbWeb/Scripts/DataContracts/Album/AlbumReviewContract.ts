
import UserApiContract from '../User/UserApiContract';

//namespace vdb.dataContracts.albums {

	export default interface AlbumReviewContract {

		date: string;

		id?: number;

		languageCode: string;

		text: string;

		title: string;

		user: UserApiContract;

	}

//}