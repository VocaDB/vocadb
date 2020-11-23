using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtenders
{
	public static class AlbumQueryableExtender
	{
		public static IQueryable<Album> OrderByReleaseDate(this IQueryable<Album> query, SortDirection direction)
		{
			return query
				.OrderBy(a => a.OriginalRelease.ReleaseDate.Year, direction)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Month, direction)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Day, direction);
		}

		public static IQueryable<Album> OrderBy(this IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference)
		{
			switch (sortRule)
			{
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.CollectionCount:
					return criteria.OrderByDescending(a => a.UserCollections.Count);
				case AlbumSortRule.ReleaseDate:
					return criteria.OrderByReleaseDate(SortDirection.Descending);
				case AlbumSortRule.ReleaseDateWithNulls:
					return criteria.OrderByReleaseDate(SortDirection.Descending);
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
			this IQueryable<Album> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference, SortDirection? direction)
		{
			switch (sortRule)
			{
				case EntrySortRule.Name:
					return FindHelpers.AddNameOrder(query, languagePreference);
				case EntrySortRule.AdditionDate:
					return query.OrderByDescending(a => a.CreateDate);
				case EntrySortRule.ActivityDate:
					return query.OrderByReleaseDate(direction ?? SortDirection.Descending);
			}

			return query;
		}

		public static IQueryable<Album> WhereArtistHasType(this IQueryable<Album> query, ArtistType artistType)
		{
			return query.WhereArtistHasType<Album, ArtistForAlbum>(artistType);
		}

		public static IQueryable<Album> WhereDraftsOnly(this IQueryable<Album> query, bool draftsOnly)
		{
			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);
		}

		public static IQueryable<Album> WhereHasArtist(this IQueryable<Album> query, int artistId)
		{
			if (artistId == 0)
				return query;

			return query.WhereHasArtist<Album, ArtistForAlbum>(artistId, false, false);
		}

		public static IQueryable<Album> WhereHasArtistParticipationStatus(
			this IQueryable<Album> query,
			ArtistParticipationQueryParams queryParams,
			EntryIdsCollection artistIds,
			IEntityLoader<Artist> artistGetter)
		{
			var various = Model.Helpers.ArtistHelper.VariousArtists;
			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;
			var artistId = artistIds.Primary;

			return EntryWithArtistsQueryableExtender.WhereHasArtistParticipationStatus(new ArtistParticipationQueryParams<Album, ArtistForAlbum>(query, queryParams, artistIds, artistGetter,
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != various),
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && (a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == various))
			));
		}

		public static IQueryable<Album> WhereHasBarcode(this IQueryable<Album> query, string barcode)
		{
			if (string.IsNullOrEmpty(barcode))
				return query;

			return query.Where(a => a.Identifiers.Any(i => i.Value == barcode));
		}

		public static IQueryable<Album> WhereHasLinkWithCategory(this IQueryable<Album> query, WebLinkCategory category)
		{
			return query.Where(m => m.WebLinks.Any(l => l.Category == category));
		}

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
		public static IQueryable<Album> WhereHasName(this IQueryable<Album> query, SearchTextQuery textQuery, bool allowCatNum = false)
		{
			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;
			var expression = EntryWithNamesQueryableExtender.WhereHasNameExpression<Album, AlbumName>(textQuery);

			if (allowCatNum)
			{
				expression = expression.Or(q => q.OriginalRelease.CatNum != null && q.OriginalRelease.CatNum.Contains(nameFilter));
			}

			return query.Where(expression);
		}

		public static IQueryable<Album> WhereHasReleaseDate(this IQueryable<Album> criteria)
		{
			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);
		}

		public static IQueryable<Album> WhereHasTag(this IQueryable<Album> query, string tagName)
		{
			return query.WhereHasTag<Album, AlbumTagUsage>(tagName);
		}

		public static IQueryable<Album> WhereHasTags(this IQueryable<Album> query, string[] tagName)
		{
			return query.WhereHasTags<Album, AlbumTagUsage>(tagName);
		}

		public static IQueryable<Album> WhereHasTags(this IQueryable<Album> query, int[] tagId, bool childTags = false)
		{
			return query.WhereHasTags<Album, AlbumTagUsage>(tagId, childTags);
		}

		public static IQueryable<Album> WhereHasType(this IQueryable<Album> query, DiscType albumType)
		{
			if (albumType == DiscType.Unknown)
				return query;

			return query.Where(m => m.DiscType == albumType);
		}

		public static IQueryable<Album> WhereMatchFilter(this IQueryable<Album> query, AdvancedSearchFilter filter)
		{
			switch (filter.FilterType)
			{
				case AdvancedFilterType.ArtistType:
					{
						var param = EnumVal<ArtistType>.Parse(filter.Param);
						return WhereArtistHasType(query, param);
					}
				case AdvancedFilterType.NoCoverPicture:
					{
						return query.Where(a => a.CoverPictureMime == null || a.CoverPictureMime == string.Empty);
					}
				case AdvancedFilterType.HasStoreLink:
					{
						return query.WhereHasLinkWithCategory(WebLinkCategory.Commercial);
					}
				case AdvancedFilterType.HasTracks:
					{
						return query.Where(a => filter.Negate != a.AllSongs.Any(s => s.Song == null || !s.Song.Deleted));
					}
				case AdvancedFilterType.WebLink:
					{
						return query.WhereHasLink<Album, AlbumWebLink>(filter.Param);
					}
			}

			return query;
		}

		public static IQueryable<Album> WhereMatchFilters(this IQueryable<Album> query, IEnumerable<AdvancedSearchFilter> filters)
		{
			return filters?.Aggregate(query, WhereMatchFilter) ?? query;
		}

		public static IQueryable<Album> WhereReleaseDateIsAfter(this IQueryable<Album> query, DateTime? beginDateNullable)
		{
			if (!beginDateNullable.HasValue)
				return query;

			var beginDate = beginDateNullable.Value;

			return query.Where(a => a.OriginalRelease.ReleaseDate.Year > beginDate.Year
				|| (a.OriginalRelease.ReleaseDate.Year == beginDate.Year && a.OriginalRelease.ReleaseDate.Month > beginDate.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == beginDate.Year
					&& a.OriginalRelease.ReleaseDate.Month == beginDate.Month
					&& a.OriginalRelease.ReleaseDate.Day >= beginDate.Day));
		}

		public static IQueryable<Album> WhereReleaseDateIsBefore(this IQueryable<Album> query, DateTime? endDateNullable)
		{
			if (!endDateNullable.HasValue)
				return query;

			var endDate = endDateNullable.Value;

			return query.Where(a => a.OriginalRelease.ReleaseDate.Year < endDate.Year
				|| (a.OriginalRelease.ReleaseDate.Year == endDate.Year && a.OriginalRelease.ReleaseDate.Month < endDate.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == endDate.Year
					&& a.OriginalRelease.ReleaseDate.Month == endDate.Month
					&& a.OriginalRelease.ReleaseDate.Day < endDate.Day));
		}

		/// <summary>
		/// Makes sure that the query is filtered by restrictions of the sort rule.
		/// This can be used to separate the filtering from the actual sorting, when sorting is not needed (for example, only count is needed).
		/// </summary>
		public static IQueryable<Album> WhereSortBy(this IQueryable<Album> query, AlbumSortRule sort)
		{
			switch (sort)
			{
				case AlbumSortRule.ReleaseDate:
					return query.WhereHasReleaseDate();

				default:
					return query;
			}
		}
	}
}
