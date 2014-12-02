using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public static class FavoriteSongForUserQueryableExtender {

		public static IQueryable<FavoriteSongForUser> WhereHasRating(this IQueryable<FavoriteSongForUser> query, SongVoteRating rating) {

			if (rating == SongVoteRating.Nothing)
				return query;

			return query.Where(q => q.Rating == rating);

		}

	}
}
