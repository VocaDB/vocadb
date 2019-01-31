
namespace vdb.dataContracts.albums {

	export interface AlbumReviewContract {

		date: string;

		text: string;

		title: string;

		user: user.UserApiContract;

	}

}