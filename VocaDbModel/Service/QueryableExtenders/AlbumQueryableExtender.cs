using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class AlbumQueryableExtender {

		public static IQueryable<Album> OrderByReleaseDate(this IQueryable<Album> query) {
			
			return query
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);

		} 

		public static IQueryable<Album> OrderBy(this IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference) {
			
			switch (sortRule) {
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.CollectionCount:
					return criteria.OrderByDescending(a => a.UserCollections.Count);
				case AlbumSortRule.ReleaseDate:
					return criteria.OrderByReleaseDate();
				case AlbumSortRule.ReleaseDateWithNulls:
					return criteria.OrderByReleaseDate();
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.RatingTotal:
					return criteria.OrderByDescending(a => a.RatingTotal)
						.ThenByDescending(a => a.RatingAverageInt);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;

		}

		public static IQueryable<Album> OrderBy(
			this IQueryable<Album> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case EntrySortRule.Name:
					return FindHelpers.AddNameOrder(query, languagePreference);
				case EntrySortRule.AdditionDate:
					return query.OrderByDescending(a => a.CreateDate);
			}

			return query;

		}

		public static IQueryable<Album> WhereDraftsOnly(this IQueryable<Album> query, bool draftsOnly) {

			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);

		}

		public static IQueryable<Album> WhereHasArtistParticipationStatus(
			this IQueryable<Album> query, EntryIdsCollection artistIds, ArtistAlbumParticipationStatus participation, 
			bool childVoicebanks,
			Func<int, Artist> artistGetter) {

			var various = Model.Helpers.ArtistHelper.VariousArtists;
			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;
			var artistId = artistIds.Primary;

			return EntryWithArtistsQueryableExtender.WhereHasArtistParticipationStatus(new ArtistParticipationQueryParams<Album, ArtistForAlbum>(query, artistIds, participation,
				childVoicebanks, artistGetter,
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != various),
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && (a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == various))
			));

		}

		public static IQueryable<Album> WhereHasBarcode(this IQueryable<Album> query, string barcode) {

			if (string.IsNullOrEmpty(barcode))
				return query;

			return query.Where(a => a.Identifiers.Any(i => i.Value == barcode));

		}

		// TODO: should be combined with common name query somehow, but NH is making it hard
		/// <summary>
		/// Filters an artist query by a name query.
		/// </summary>
		/// <param name="query">Artist query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Album> WhereHasName(this IQueryable<Album> query, SearchTextQuery textQuery, bool allowCatNum = false) {

			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => 
						(allowCatNum && m.OriginalRelease.CatNum != null && m.OriginalRelease.CatNum.Contains(nameFilter)) ||
						m.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => 
						(allowCatNum && m.OriginalRelease.CatNum != null && m.OriginalRelease.CatNum.StartsWith(nameFilter)) ||
						m.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					Expression<Func<Album, bool>> catNumExp = (q => allowCatNum && q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter));

					Expression<Func<Album, bool>> nameExp = (q => q.Names.Names.Any(n => n.Value.Contains(words[0])));

					foreach (var word in words.Skip(1).Take(FindHelpers.MaxSearchWords)) {
						var temp = word;
						nameExp = nameExp.And((q => q.Names.Names.Any(n => n.Value.Contains(temp))));
					}

					return query.Where(catNumExp.Or(nameExp));

			}

			return query;

		}

		public static IQueryable<Album> WhereHasReleaseDate(this IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

		public static IQueryable<Album> WhereHasTag(this IQueryable<Album> query, string tagName) {

			return query.WhereHasTag<Album, AlbumTagUsage>(tagName);

		}

		public static IQueryable<Album> WhereHasTags(this IQueryable<Album> query, string[] tagName) {

			return query.WhereHasTags<Album, AlbumTagUsage>(tagName);

		}

		public static IQueryable<Album> WhereHasType(this IQueryable<Album> query, DiscType albumType) {

			if (albumType == DiscType.Unknown)
				return query;

			return query.Where(m => m.DiscType == albumType);

		}

		public static IQueryable<Album> WhereIsDeleted(this IQueryable<Album> query, bool deleted) {
			
			return query.Where(m => m.Deleted == deleted);

		}

		/// <summary>
		/// Makes sure that the query is filtered by restrictions of the sort rule.
		/// This can be used to separate the filtering from the actual sorting, when sorting is not needed (for example, only count is needed).
		/// </summary>
		public static IQueryable<Album> WhereSortBy(this IQueryable<Album> query, AlbumSortRule sort) {

			switch (sort) {
				case AlbumSortRule.ReleaseDate:
					return query.WhereHasReleaseDate();

				default:
					return query;
			}

		}

	}
}
