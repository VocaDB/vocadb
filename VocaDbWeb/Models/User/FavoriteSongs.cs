using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Web.Models.User {

	public class FavoriteSongs {

		public FavoriteSongs() {
			GroupByRating = true;
			Rating = SongVoteRating.Nothing;
			Sort = RatedSongForUserSortRule.Name;
		}

		public FavoriteSongs(UserContract user, SongVoteRating rating, RatedSongForUserSortRule? sort, bool? groupByRating)
			: this() {

			GroupByRating = groupByRating;
			Rating = rating;
			Sort = sort;
			User = user;

		}

		public bool? GroupByRating { get; set; }

		public SongVoteRating Rating { get; set; }

		public RatedSongForUserSortRule? Sort { get; set; }

		public UserContract User { get; set; }

	}

}