using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class FavoriteSongForUserQueryableExtender {

		public static IQueryable<FavoriteSongForUser> OrderBy(this IQueryable<FavoriteSongForUser> query, RatedSongForUserSortRule sortRule, ContentLanguagePreference languagePreference) {

			if (sortRule == RatedSongForUserSortRule.RatingDate)
				return query.OrderByDescending(s => s.Date);

			return query.OrderBy((SongSortRule)sortRule, languagePreference);

		}

		public static IQueryable<FavoriteSongForUser> WhereHasRating(this IQueryable<FavoriteSongForUser> query, SongVoteRating rating) {

			if (rating == SongVoteRating.Nothing)
				return query;

			return query.Where(q => q.Rating == rating);

		}

	}

	public enum RatedSongForUserSortRule {

		None,

		Name,

		AdditionDate,

		PublishDate,

		FavoritedTimes,

		RatingScore,

		RatingDate

	}

}
