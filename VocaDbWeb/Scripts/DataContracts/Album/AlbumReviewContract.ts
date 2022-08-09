import { UserApiContract } from '@/DataContracts/User/UserApiContract';

export interface AlbumReviewContract {
	date: string;

	id?: number;

	languageCode: string;

	text: string;

	title: string;

	user: UserApiContract;
}
