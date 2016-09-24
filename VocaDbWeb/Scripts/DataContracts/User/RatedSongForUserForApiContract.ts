
namespace vdb.dataContracts {
	
	export interface RatedSongForUserForApiContract {

		rating: string;

		song?: SongApiContract;

		user?: user.UserApiContract;

	}

}