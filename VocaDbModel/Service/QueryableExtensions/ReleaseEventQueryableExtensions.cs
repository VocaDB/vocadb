using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions;

public static class ReleaseEventQueryableExtensions
{
	public static IOrderedQueryable<ReleaseEvent> OrderByDate(this IQueryable<ReleaseEvent> query, SortDirection? direction)
	{
		return query.OrderBy(e => e.Date.DateTime, direction ?? SortDirection.Descending);
	}

	/// <summary>
	/// Sort query.
	/// </summary>
	/// <param name="query">Query. Cannot be null.</param>
	/// <param name="sortRule">Sort rule.</param>
	/// <param name="direction">Sort direction. If null, default direction is used.</param>
	/// <returns>Sorted query. Cannot be null.</returns>
	public static IQueryable<ReleaseEvent> OrderBy(this IQueryable<ReleaseEvent> query, EventSortRule sortRule, ContentLanguagePreference languagePreference, SortDirection? direction) => sortRule switch
	{
		EventSortRule.Date => query.OrderByDate(direction),
		EventSortRule.AdditionDate => query.OrderBy(e => e.CreateDate, direction ?? SortDirection.Descending),
		EventSortRule.Name => query.OrderByName(languagePreference),
		EventSortRule.SeriesName => query.OrderBySeriesName(languagePreference),
		EventSortRule.VenueName => query.OrderByVenueName(languagePreference),
		_ => query,
	};

	public static IQueryable<ReleaseEvent> OrderBy(
		this IQueryable<ReleaseEvent> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference, SortDirection? direction) => sortRule switch
	{
		EntrySortRule.Name => query.OrderByName(languagePreference),
		EntrySortRule.AdditionDate => query.OrderBy(e => e.CreateDate, direction ?? SortDirection.Descending),
		EntrySortRule.ActivityDate => query.OrderByDate(direction),
		_ => query,
	};

	public static IOrderedQueryable<ReleaseEvent> OrderByName(this IQueryable<ReleaseEvent> query, ContentLanguagePreference languagePreference)
	{
		return query.OrderByEntryName(languagePreference);
	}

	public static IOrderedQueryable<ReleaseEvent> OrderBySeriesName(this IQueryable<ReleaseEvent> query, ContentLanguagePreference languagePreference)
	{
		var ordered = languagePreference switch
		{
			ContentLanguagePreference.English => query.OrderBy(e => e.Series.Names.SortNames.English),
			ContentLanguagePreference.Romaji => query.OrderBy(e => e.Series.Names.SortNames.Romaji),
			_ => query.OrderBy(e => e.Series.Names.SortNames.Japanese),
		};
		return ordered.ThenBy(e => e.SeriesNumber);
	}

	public static IOrderedQueryable<ReleaseEvent> OrderByVenueName(this IQueryable<ReleaseEvent> query, ContentLanguagePreference languagePreference)
	{
		var ordered = languagePreference switch
		{
			ContentLanguagePreference.English => query.OrderBy(e => e.Venue.Names.SortNames.English),
			ContentLanguagePreference.Romaji => query.OrderBy(e => e.Venue.Names.SortNames.Romaji),
			_ => query.OrderBy(e => e.Venue.Names.SortNames.Japanese)
		};

		return ordered.ThenBy(e => e.VenueName);
	}

	public static IQueryable<ReleaseEvent> WhereDateIsBetween(this IQueryable<ReleaseEvent> query, DateTime? begin, DateTime? end)
	{
		if (begin.HasValue && end.HasValue)
			return query.Where(e => e.Date.DateTime != null && e.Date.DateTime >= begin && e.Date.DateTime < end);

		if (begin.HasValue)
			return query.Where(e => e.Date.DateTime != null && e.Date.DateTime >= begin);

		if (end.HasValue)
			return query.Where(e => e.Date.DateTime != null && e.Date.DateTime < end);

		return query;
	}

	// Returns release events which are happening at date or after date. Mainly used for the "Happening now" section on the frontpage.
	public static IQueryable<ReleaseEvent> FromDateOnwards(this IQueryable<ReleaseEvent> query, DateTime? date)
	{
		if (!date.HasValue) return query;
		
		return query.Where(e => (e.EndDate.DateTime != null && e.EndDate.DateTime > date) || (e.Date.DateTime != null && e.Date.DateTime >= date));
	}

	public static IQueryable<ReleaseEvent> WhereHasArtists(this IQueryable<ReleaseEvent> query, EntryIdsCollection artistIds, bool childVoicebanks, bool includeMembers)
	{
		if (!artistIds.HasAny)
			return query;

		return query.WhereHasArtists<ReleaseEvent, ArtistForEvent>(artistIds, childVoicebanks, includeMembers);
	}

	public static IQueryable<ReleaseEvent> WhereHasCategory(this IQueryable<ReleaseEvent> query, EventCategory category)
	{
		if (category == EventCategory.Unspecified)
			return query;

		return query.Where(e => (e.Series != null && e.Series.Category == category) || (e.Series == null && e.Category == category));
	}

	public static IQueryable<ReleaseEvent> WhereHasName(this IQueryable<ReleaseEvent> query, SearchTextQuery textQuery)
	{
		return query.WhereHasNameGeneric<ReleaseEvent, EventName>(textQuery);
	}

	public static IQueryable<ReleaseEvent> WhereHasSeries(this IQueryable<ReleaseEvent> query, int seriesId)
	{
		if (seriesId == 0)
			return query;

		return query.Where(e => e.Series.Id == seriesId);
	}

	public static IQueryable<ReleaseEvent> WhereHasTags(this IQueryable<ReleaseEvent> query, int[]? tagId, bool childTags = false)
	{
		return query.WhereHasTags<ReleaseEvent, EventTagUsage>(tagId, childTags);
	}

	public static IQueryable<ReleaseEvent> WhereInUserCollection(this IQueryable<ReleaseEvent> query, int userId)
	{
		if (userId == 0)
			return query;

		return query.Where(s => s.Users.Any(a => a.User.Id == userId));
	}
}

public enum EventSortRule
{
	None,

	Name,

	/// <summary>
	/// Event date
	/// </summary>
	Date,

	AdditionDate,

	SeriesName,

	VenueName
}
