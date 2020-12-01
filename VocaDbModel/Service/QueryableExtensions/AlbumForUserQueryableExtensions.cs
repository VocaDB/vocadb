using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class AlbumForUserQueryableExtensions
	{
		public static IOrderedQueryable<AlbumForUser> OrderByAlbumName(this IQueryable<AlbumForUser> criteria, ContentLanguagePreference languagePreference)
		{
			switch (languagePreference)
			{
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Album.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Album.Names.SortNames.English);
				default:
					return criteria.OrderBy(e => e.Album.Names.SortNames.Romaji);
			}
		}

		public static IQueryable<AlbumForUser> OrderBy(this IQueryable<AlbumForUser> query, AlbumSortRule sortRule, ContentLanguagePreference languagePreference)
		{
			switch (sortRule)
			{
				case AlbumSortRule.Name:
					return OrderByAlbumName(query, languagePreference);
				case AlbumSortRule.CollectionCount:
					return query.OrderByDescending(a => a.Album.UserCollections.Count);
				case AlbumSortRule.ReleaseDate:
					return query
						.WhereHasReleaseDate()
						.OrderByReleaseDate();
				case AlbumSortRule.ReleaseDateWithNulls:
					return query.OrderByReleaseDate();
				case AlbumSortRule.AdditionDate:
					return query.OrderByDescending(a => a.Album.CreateDate);
				case AlbumSortRule.RatingAverage:
					return query.OrderByDescending(a => a.Album.RatingAverageInt)
						.ThenByDescending(a => a.Album.RatingCount);
				case AlbumSortRule.RatingTotal:
					return query.OrderByDescending(a => a.Album.RatingTotal)
						.ThenByDescending(a => a.Album.RatingAverageInt);
				case AlbumSortRule.NameThenReleaseDate:
					return OrderByAlbumName(query, languagePreference)
						.ThenBy(a => a.Album.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.Album.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.Album.OriginalRelease.ReleaseDate.Day);
			}

			return query;
		}

		public static IQueryable<AlbumForUser> OrderByReleaseDate(this IQueryable<AlbumForUser> query)
		{
			return query
				.OrderByDescending(a => a.Album.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.Album.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.Album.OriginalRelease.ReleaseDate.Day);
		}

		public static IQueryable<AlbumForUser> WhereHasArtist(this IQueryable<AlbumForUser> query, int artistId)
		{
			if (artistId == 0)
				return query;

			return query.Where(s => s.Album.AllArtists.Any(a => a.Artist.Id == artistId));
		}

		public static IQueryable<AlbumForUser> WhereHasCollectionStatus(this IQueryable<AlbumForUser> query, PurchaseStatus[] statuses)
		{
			if (statuses == null || !statuses.Any())
				return query;

			if (statuses.Length == 1)
			{
				var s = statuses[0];
				return query.Where(a => a.PurchaseStatus == s);
			}
			else
			{
				return query.Where(a => statuses.Contains(a.PurchaseStatus));
			}
		}

		/// <summary>
		/// Filters a song link query by a name query.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		/// <remarks>
		/// Note: this code should be optimized after switching to EF.
		/// Cannot be optimized as is for NH.
		/// </remarks>
		public static IQueryable<AlbumForUser> WhereHasName(this IQueryable<AlbumForUser> query, SearchTextQuery textQuery)
		{
			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode)
			{
				case NameMatchMode.Exact:
					return query.Where(m => m.Album.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Album.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Album.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					switch (words.Length)
					{
						case 1:
							query = query.Where(q => q.Album.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q =>
								q.Album.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q =>
								q.Album.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q =>
								q.Album.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q =>
								q.Album.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q =>
								q.Album.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Album.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;
			}

			return query;
		}

		public static IQueryable<AlbumForUser> WhereHasReleaseDate(this IQueryable<AlbumForUser> criteria)
		{
			return criteria.Where(a => a.Album.OriginalRelease.ReleaseDate.Year != null
				&& a.Album.OriginalRelease.ReleaseDate.Month != null
				&& a.Album.OriginalRelease.ReleaseDate.Day != null);
		}

		public static IQueryable<AlbumForUser> WhereHasReleaseEvent(this IQueryable<AlbumForUser> query, int releaseEventId)
		{
			if (releaseEventId == 0)
				return query;

			return query.Where(s => s.Album.OriginalRelease.ReleaseEvent.Id == releaseEventId);
		}
	}
}
