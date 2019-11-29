
import UserApiContract from '../User/UserApiContract';

//namespace vdb.dataContracts.albums {

	export interface AlbumReviewContract {

		date: string;

		id?: number;

		languageCode: string;

		text: string;

		title: string;

		user: UserApiContract;

	}

//}