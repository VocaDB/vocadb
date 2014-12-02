using System;
using System.Linq;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Helpers {

	public static class UserHelper {

		public static int GetLevel(int power) {

			if (power <= 0)
				return 0;

			return (int)Math.Log(power, Math.E);

		}

		public static int GetPower(UserDetailsContract detailsContract, int ownedAlbumCount, int albumRatingCount) {

			ParamIs.NotNull(() => detailsContract);
			var songListCount = detailsContract.SongLists.Count();

			var power =
				detailsContract.EditCount / 4
				+ detailsContract.SubmitCount / 2
				+ detailsContract.TagVotes * 2
				+ detailsContract.AlbumCollectionCount * 2
				+ ownedAlbumCount * 5
				+ albumRatingCount * 3
				+ detailsContract.FavoriteSongCount * 2
				+ detailsContract.CommentCount * 4
				+ songListCount * 5
				+ (detailsContract.EmailVerified ? 100 : 0);

			return power;

		}

	}

}
