using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class ArtistLinkQueryableExtender {

		public static IQueryable<T> OrderBy<T>(
			this IQueryable<T> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) where T : IArtistLink {

			switch (sortRule) {
				case ArtistSortRule.Name:
					return criteria.OrderByName(languagePreference);
				case ArtistSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.Artist.CreateDate);
				case ArtistSortRule.AdditionDateAsc:
					return criteria.OrderBy(a => a.Artist.CreateDate);
				case ArtistSortRule.SongCount:
					return criteria.OrderByDescending(a => a.Artist.AllSongs.Count());
				case ArtistSortRule.SongRating:
					return criteria.OrderByDescending(a => a.Artist.AllSongs
						.Where(s => !s.Song.Deleted)
						.Sum(s => s.Song.RatingScore));
			}

			return criteria;

		}

		public static IOrderedQueryable<T> OrderByName<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IArtistLink {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Artist.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Artist.Names.SortNames.English);
				default:
					return criteria.OrderBy(e => e.Artist.Names.SortNames.Romaji);
			}

		}

	}

}
