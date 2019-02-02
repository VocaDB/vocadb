
namespace vdb.dataContracts.albums {

	export interface AlbumReviewContract {

		date: string;

		id?: number;

		languageCode: string;

		text: string;

		title: string;

		user: user.UserApiContract;

	}

}