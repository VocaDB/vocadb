using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service.QueryableExtensions;

public static class ArtistQueryableExtensions
{
	public static IQueryable<ArtistName> WhereArtistNameIs(this IQueryable<ArtistName> query, ArtistSearchTextQuery textQuery)
	{
		var canonizedName = textQuery.Query;

		if (textQuery.IsExact)
		{
			return query.Where(m =>
				m.Value == canonizedName ||
				m.Value == $"{canonizedName}P" ||
				m.Value == $"{canonizedName}-P"
			);
		}
		else
		{
			return query.WhereEntryNameIs(textQuery);
		}
	}

	public static IQueryable<ArtistName> WhereArtistTypeIn(this IQueryable<ArtistName> queryable, ArtistType[]? types)
	{
		if (types == null || !types.Any())
			return queryable;

		return queryable.Where(n => types.Contains(n.Artist.ArtistType));
	}

	private static IQueryable<Artist> OrderBySongRating(this IQueryable<Artist> criteria)
	{
		return criteria.OrderByDescending(a => a.AllSongs.Where(s => !s.Song.Deleted).Sum(s => s.Song.RatingScore));
	}

	private static IQueryable<Artist> ThenBySongRating(this IOrderedQueryable<Artist> criteria)
	{
		return criteria.ThenByDescending(a => a.AllSongs.Where(s => !s.Song.Deleted).Sum(s => s.Song.RatingScore));
	}

	private static IOrderedQueryable<Artist> OrderByArtistType(this IQueryable<Artist> query)
	{
		return query.OrderByDescending(a =>
			a.ArtistType == ArtistType.Vocaloid ||
			a.ArtistType == ArtistType.UTAU ||
			a.ArtistType == ArtistType.CeVIO ||
			a.ArtistType == ArtistType.OtherVoiceSynthesizer ||
			a.ArtistType == ArtistType.SynthesizerV
		);
	}

	public static IQueryable<Artist> OrderBy(
		this IQueryable<Artist> criteria,
		ArtistSortRule sortRule,
		ContentLanguagePreference languagePreference
	)
	{
		return sortRule switch
		{
			ArtistSortRule.Name => FindHelpers.AddNameOrder(criteria, languagePreference),
			ArtistSortRule.AdditionDate => criteria.OrderByDescending(a => a.CreateDate),
			ArtistSortRule.AdditionDateAsc => criteria.OrderBy(a => a.CreateDate),
			ArtistSortRule.ReleaseDate => OrderByReleaseDate(criteria, SortDirection.Descending),
			ArtistSortRule.SongCount => criteria.OrderByDescending(a => a.AllSongs.Count(s => !s.Song.Deleted)),
			ArtistSortRule.SongRating => criteria.OrderBySongRating(),
			ArtistSortRule.FollowerCount => criteria.OrderByDescending(a => a.Users.Count),
			ArtistSortRule.ArtistType => criteria.OrderByArtistType().ThenBySongRating(),
			_ => criteria,
		};
	}

	public static IQueryable<Artist> OrderBy(
		this IQueryable<Artist> query,
		EntrySortRule sortRule,
		ContentLanguagePreference languagePreference
	)
	{
		return sortRule switch
		{
			EntrySortRule.Name => FindHelpers.AddNameOrder(query, languagePreference),
			EntrySortRule.AdditionDate => query.OrderByDescending(a => a.CreateDate),
			_ => query,
		};
	}

	public static IQueryable<Artist> OrderByReleaseDate(this IQueryable<Artist> criteria, SortDirection direction)
	{
		return criteria
			.OrderBy(a => a.ReleaseDate, direction)
			.ThenBy(a => a.CreateDate, direction);
	}

	public static IQueryable<Artist> WhereDraftsOnly(this IQueryable<Artist> query, bool draftsOnly)
	{
		if (!draftsOnly)
			return query;

		return query.Where(a => a.Status == EntryStatus.Draft);
	}

	/// <summary>
	/// Filters an artist query by external link URL.
	/// </summary>
	/// <param name="query">Artists query. Cannot be null.</param>
	/// <param name="extLinkUrl">
	/// External link URL, for example http://www.nicovideo.jp/mylist/6667938.
	/// </param>
	/// <returns>Filtered query. Cannot be null.</returns>
	public static IQueryable<Artist> WhereHasExternalLinkUrl(this IQueryable<Artist> query, string? extLinkUrl)
	{
		if (string.IsNullOrEmpty(extLinkUrl) || extLinkUrl.Length <= 1)
			return query;

		return query.WhereHasLink<Artist, ArtistWebLink>(extLinkUrl, WebLinkVariationTypes.All);
	}

	/// <summary>
	/// Filters an artist query by a name query.
	/// </summary>
	/// <param name="query">Artist query. Cannot be null.</param>
	/// <param name="textQuery">Textual filter. Cannot be null.</param>
	/// <returns>Filtered query. Cannot be null.</returns>
	public static IQueryable<Artist> WhereHasName(this IQueryable<Artist> query, ArtistSearchTextQuery textQuery)
	{
		return query.WhereHasNameGeneric<Artist, ArtistName>(textQuery);
	}

	/// <summary>
	/// Filters query by canonized artist name.
	/// This means that any P suffixes are ignored.
	/// </summary>
	/// <param name="query">Artist query. Cannot be null.</param>
	/// <param name="textQuery">Textual filter. Cannot be null.</param>
	/// <returns>Filtered query. Cannot be null.</returns>
	public static IQueryable<Artist> WhereHasName_Canonized(this IQueryable<Artist> query, ArtistSearchTextQuery textQuery)
	{
		if (textQuery.IsEmpty)
			return query;

		var canonizedName = textQuery.Query;

		if (textQuery.IsExact)
		{
			return query.Where(m => m.Names.Names.Any(n =>
				n.Value == canonizedName ||
				n.Value == $"{canonizedName}P" ||
				n.Value == $"{canonizedName}-P"
			));
		}
		else
		{
			return query.WhereHasName(textQuery);
		}
	}

	public static IQueryable<Artist> WhereHasTag(this IQueryable<Artist> query, string? tagName)
	{
		return query.WhereHasTag<Artist, ArtistTagUsage>(tagName);
	}

	public static IQueryable<Artist> WhereHasTags(this IQueryable<Artist> query, string[]? tagName)
	{
		return query.WhereHasTags<Artist, ArtistTagUsage>(tagName);
	}

	public static IQueryable<Artist> WhereHasTags(this IQueryable<Artist> query, int[]? tagId, bool childTags = false)
	{
		return query.WhereHasTags<Artist, ArtistTagUsage>(tagId, childTags);
	}

	public static IQueryable<Artist> WhereHasType(this IQueryable<Artist> query, ArtistType[] artistTypes)
	{
		if (!artistTypes.Any())
			return query;

		return query.Where(m => artistTypes.Contains(m.ArtistType));
	}

	public static IQueryable<Artist> WhereIdIs(this IQueryable<Artist> query, int id)
	{
		if (id == 0)
			return query;

		return query.Where(m => m.Id == id);
	}

	public static IQueryable<Artist> WhereIsFollowedByUser(this IQueryable<Artist> query, int userId)
	{
		if (userId == 0)
			return query;

		query = query.Where(s => s.Users.Any(a => a.User.Id == userId));

		return query;
	}

	public static IQueryable<Artist> WhereAllowBaseVoicebanks(this IQueryable<Artist> query, bool allowed)
	{
		if (allowed)
			return query;

		return query.Where(a => a.BaseVoicebank == null);
	}

	public static IQueryable<Artist> WhereMatchFilter(this IQueryable<Artist> query, AdvancedSearchFilter filter)
	{
		switch (filter.FilterType)
		{
			case AdvancedFilterType.HasUserAccount:
				{
					return query.Where(a => a.OwnerUsers.Any());
				}
			case AdvancedFilterType.RootVoicebank:
				{
					return filter.Negate ? query.Where(a => a.BaseVoicebank != null) : query.Where(a => ArtistHelper.VoiceSynthesizerTypes.Contains(a.ArtistType) && a.BaseVoicebank == null);
				}
			case AdvancedFilterType.VoiceProvider:
				{
					var param = EnumVal<ArtistType>.ParseSafe(filter.Param, ArtistType.Unknown);
					return param == ArtistType.Unknown ?
						query.Where(a => a.AllMembers.Any(m => m.LinkType == ArtistLinkType.VoiceProvider)) :
						query.Where(a => a.AllMembers.Any(m => m.LinkType == ArtistLinkType.VoiceProvider && m.Member.ArtistType == param));
				}
			case AdvancedFilterType.WebLink:
				{
					return query.WhereHasLink<Artist, ArtistWebLink>(filter.Param);
				}
		}

		return query;
	}

	public static IQueryable<Artist> WhereMatchFilters(this IQueryable<Artist> query, IEnumerable<AdvancedSearchFilter>? filters)
	{
		return filters?.Aggregate(query, WhereMatchFilter) ?? query;
	}

	public static IQueryable<Artist> WhereHasLanguage(this IQueryable<Artist> query, string? language)
	{
		if (language == null)
		{
			return query;
		}

		return query.Where(s => s.CultureCodes.Any(c => c.CultureCode == language));
	}

	public static IQueryable<Artist> WhereHasLanguages(this IQueryable<Artist> query, string[]? languages)
	{
		if (languages == null || !languages.Any())
		{
			return query;
		}

		return languages.Aggregate(query, WhereHasLanguage);
	}
}
