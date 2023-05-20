using System.Linq.Expressions;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions;

/// <summary>
/// Query extension methods for <see cref="ISongLink"/>.
/// </summary>
public static class SongLinkQueryableExtensions
{
	public static IQueryable<T> OrderByPublishDate<T>(this IQueryable<T> criteria, SortDirection direction) where T : ISongLink
	{
		return criteria.OrderBy(a => a.Song.PublishDate, direction)
			.ThenBy(a => a.Song.CreateDate, direction);
	}

	public static IOrderedQueryable<T> OrderBySongName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference)
		where T : ISongLink => languagePreference switch
		{
			ContentLanguagePreference.Japanese => query.OrderBy(e => e.Song.Names.SortNames.Japanese),
			ContentLanguagePreference.English => query.OrderBy(e => e.Song.Names.SortNames.English),
			_ => query.OrderBy(e => e.Song.Names.SortNames.Romaji),
		};

	public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, SongSortRule sortRule, ContentLanguagePreference languagePreference)
		where T : ISongLink => sortRule switch
		{
			SongSortRule.Name => query.OrderBySongName(languagePreference),
			SongSortRule.AdditionDate => query.OrderByDescending(a => a.Song.CreateDate),
			SongSortRule.FavoritedTimes => query.OrderByDescending(a => a.Song.FavoritedTimes),
			SongSortRule.RatingScore => query.OrderByDescending(a => a.Song.RatingScore),
			SongSortRule.PublishDate => query.OrderByDescending(a => a.Song.PublishDate.DateTime),
			_ => query,
		};

	public static Expression<Func<TEntry, bool>> GetChildHasNameExpression<TEntry>(SearchTextQuery textQuery)
		where TEntry : ISongLink
	{
		var nameFilter = textQuery.Query;

		switch (textQuery.MatchMode)
		{
			case NameMatchMode.Exact:
				return m => m.Song.Names.Names.Any(n => n.Value == nameFilter);

			case NameMatchMode.Partial:
				return m => m.Song.Names.Names.Any(n => n.Value.Contains(nameFilter));

			case NameMatchMode.StartsWith:
				return m => m.Song.Names.Names.Any(n => n.Value.StartsWith(nameFilter));

			case NameMatchMode.Words:
				var words = textQuery.Words;

				Expression<Func<TEntry, bool>> exp = (q => q.Song.Names.Names.Any(n => n.Value.Contains(words[0])));

				foreach (var word in words.Skip(1).Take(10))
				{
					var temp = word;
					exp = exp.And((q => q.Song.Names.Names.Any(n => n.Value.Contains(temp))));
				}

				return exp;
		}

		return m => true;
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
	public static IQueryable<T> WhereChildHasName<T>(this IQueryable<T> query, SearchTextQuery textQuery)
		where T : ISongLink
	{
		if (textQuery.IsEmpty)
			return query;

		return query.Where(GetChildHasNameExpression<T>(textQuery));
	}

	public static IQueryable<T> WhereSongHasArtist<T>(this IQueryable<T> query, int artistId, bool childVoicebanks)
		where T : ISongLink
	{
		if (artistId == 0)
			return query;

		if (!childVoicebanks)
			return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId));
		else
			return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId
				|| a.Artist.BaseVoicebank.Id == artistId || a.Artist.BaseVoicebank.BaseVoicebank.Id == artistId
				|| a.Artist.BaseVoicebank.BaseVoicebank.BaseVoicebank.Id == artistId));
	}

	public static IQueryable<T> WhereSongHasArtists<T>(this IQueryable<T> query, EntryIdsCollection artistIds, bool childVoicebanks, LogicalGrouping grouping)
		where T : ISongLink
	{
		return grouping == LogicalGrouping.And ? WhereSongHasArtists(query, artistIds, childVoicebanks) : WhereSongHasAnyArtist(query, artistIds, childVoicebanks);
	}

	public static IQueryable<T> WhereSongHasAnyArtist<T>(this IQueryable<T> query, EntryIdsCollection artistIds, bool childVoicebanks)
		where T : ISongLink
	{
		if (!artistIds.HasAny)
			return query;

		return query.Where(s => s.Song.AllArtists.Any(a => artistIds.Ids.Contains(a.Artist.Id)));
	}

	public static IQueryable<T> WhereSongHasArtists<T>(this IQueryable<T> query, EntryIdsCollection artistIds, bool childVoicebanks)
		where T : ISongLink
	{
		if (!artistIds.HasAny)
			return query;

		return artistIds.Ids.Aggregate(query, (current, artistId) => current.WhereSongHasArtist(artistId, childVoicebanks));
	}

	public static IQueryable<T> WhereSongHasArtistWithType<T>(this IQueryable<T> query, ArtistType artistType)
		where T : ISongLink
	{
		if (artistType == ArtistType.Unknown)
			return query;

		return query.Where(s => s.Song.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == artistType));
	}

	public static IQueryable<T> WhereSongHasLyrics<T>(this IQueryable<T> query, string[]? languageCodes, bool any)
		where T : ISongLink
	{
		if (any)
		{
			return query.Where(s => s.Song.Lyrics.Any());
		}
		else if (languageCodes != null && languageCodes.Any())
		{
			foreach (var languageCode in languageCodes)
			{
				query = query.Where(s => s.Song.Lyrics.Where(l => l.RawCultureCode.Contains(languageCode)).Any());
			}
			return query;
		}
		else
		{
			return query;
		}
	}

	public static IQueryable<T> WhereSongHasPublishDate<T>(this IQueryable<T> query, bool hasPublishDate) where T : ISongLink
	{
		return hasPublishDate ? query.Where(s => s.Song.PublishDate.DateTime != null) : query.Where(s => s.Song.PublishDate.DateTime == null);
	}

	public static IQueryable<T> WhereSongIsInList<T>(this IQueryable<T> query, int listId)
		where T : ISongLink
	{
		if (listId == 0)
			return query;

		return query.Where(s => s.Song.AllListLinks.Any(l => l.List.Id == listId));
	}

	/// <summary>
	/// Filter query by PV services bit array.
	/// Song will pass the filter if ANY of the specified PV services matches.
	/// </summary>
	/// <typeparam name="T">Type of song link.</typeparam>
	/// <param name="query">Query. Cannot be null.</param>
	/// <param name="pvServices">PV services bit array. Can be null, in which case no filtering will be done.</param>
	/// <returns>Filtered query.</returns>
	public static IQueryable<T> WhereSongHasPVService<T>(this IQueryable<T> query, PVServices? pvServices)
		where T : ISongLink
	{
		if (pvServices == null)
			return query;

		return query.Where(s => (s.Song.PVServices & pvServices) != PVServices.Nothing);
	}

	public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, string? tagName)
		where T : ISongLink
	{
		if (string.IsNullOrEmpty(tagName))
			return query;

		return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Names.SortNames.English == tagName || t.Tag.Names.SortNames.Romaji == tagName || t.Tag.Names.SortNames.Japanese == tagName));
	}

	public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, int tagId)
		where T : ISongLink
	{
		if (tagId == 0)
			return query;

		return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Id == tagId));
	}

	public static IQueryable<T> WhereSongHasTags<T>(this IQueryable<T> query, int[]? tagIds)
		where T : ISongLink
	{
		if (tagIds == null || !tagIds.Any())
			return query;

		return tagIds.Aggregate(query, WhereSongHasTag);
	}

	public static IQueryable<T> WhereSongHasType<T>(this IQueryable<T> query, SongType[]? songTypes)
		where T : ISongLink
	{
		if (songTypes == null || !songTypes.Any())
			return query;

		return query.Where(m => songTypes.Contains(m.Song.SongType));
	}

	public static IQueryable<T> WhereSongPublishDateIsBetween<T>(this IQueryable<T> query, DateTime? begin, DateTime? end) where T : ISongLink
	{
		if (begin.HasValue && end.HasValue)
			return query.Where(e => e.Song.PublishDate.DateTime != null && e.Song.PublishDate.DateTime >= begin && e.Song.PublishDate.DateTime < end);

		if (begin.HasValue)
			return query.Where(e => e.Song.PublishDate.DateTime != null && e.Song.PublishDate.DateTime >= begin);

		if (end.HasValue)
			return query.Where(e => e.Song.PublishDate.DateTime != null && e.Song.PublishDate.DateTime < end);

		return query;
	}

	public static IQueryable<T> WhereMatchFilter<T>(this IQueryable<T> query, AdvancedSearchFilter? filter)
		where T : ISongLink
	{
		if (filter == null)
			return query;

		switch (filter.FilterType)
		{
			case AdvancedFilterType.ArtistType:
				{
					var param = EnumVal<ArtistType>.Parse(filter.Param);
					return WhereSongHasArtistWithType(query, param);
				}
			case AdvancedFilterType.HasAlbum:
				{
					return filter.Negate ? query.Where(s => !s.Song.AllAlbums.Any()) : query.Where(s => s.Song.AllAlbums.Any());
				}
			case AdvancedFilterType.HasOriginalMedia:
				{
					return query.Where(s => filter.Negate != s.Song.PVs.PVs.Any(pv => !pv.Disabled && pv.PVType == PVType.Original));
				}
			case AdvancedFilterType.HasMultipleVoicebanks:
				{
					return query.Where(s => s.Song.AllArtists.Count(a => !a.IsSupport && ArtistHelper.VoiceSynthesizerTypes.Contains(a.Artist.ArtistType)) > 1);
				}
			case AdvancedFilterType.HasMedia:
				{
					return query.Where(s => filter.Negate != s.Song.PVs.PVs.Any());
				}
			case AdvancedFilterType.HasPublishDate:
				{
					return query.WhereSongHasPublishDate(!filter.Negate);
				}
			case AdvancedFilterType.Lyrics:
				{
					var any = filter.Param == AdvancedSearchFilter.Any;
					var languageCodes = !any ? (filter.Param ?? string.Empty).Split(',') : null;
					return WhereSongHasLyrics(query, languageCodes, any);
				}
		}

		return query;
	}

	public static IQueryable<T> WhereMatchFilters<T>(this IQueryable<T> query, IEnumerable<AdvancedSearchFilter>? filters)
		where T : ISongLink
	{
		return filters?.Aggregate(query, WhereMatchFilter) ?? query;
	}
}
