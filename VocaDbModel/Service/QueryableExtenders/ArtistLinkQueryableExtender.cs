using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.Artists;

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
				case ArtistSortRule.FollowerCount:
					return criteria.OrderByDescending(a => a.Artist.Users.Count);
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

		public static IQueryable<T> WhereArtistHasName<T>(this IQueryable<T> query, ArtistSearchTextQuery textQuery) where T : IArtistLink {

			if (textQuery == null || textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Artist.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Artist.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Artist.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					return textQuery.Words
						.Take(FindHelpers.MaxSearchWords)
						.Aggregate(query, (q, word) => q.Where(link => link.Artist.Names.Names.Any(n => n.Value.Contains(word))));

			}

			return query;

		}

		public static IQueryable<T> WhereArtistHasType<T>(this IQueryable<T> query, ArtistType artistType) where T : IArtistLink {

			if (artistType == ArtistType.Unknown)
				return query;

			return query.Where(m => m.Artist.ArtistType == artistType);

		}

		public static IQueryable<T> WhereArtistHasType<T>(this IQueryable<T> query, ArtistType[] artistTypes) where T : IArtistLink {

			if (!artistTypes.Any())
				return query;

			return query.Where(m => artistTypes.Contains(m.Artist.ArtistType));

		}

	}

}
