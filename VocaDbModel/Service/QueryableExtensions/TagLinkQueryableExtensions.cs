using NHibernate.Criterion;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.QueryableExtensions;

public static class TagLinkQueryableExtensions
{
	public static IOrderedQueryable<T> OrderByName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference) where T : ITagLink => languagePreference switch
	{
		ContentLanguagePreference.Japanese => query.OrderBy(e => e.Tag.Names.SortNames.Japanese),
		ContentLanguagePreference.English => query.OrderBy(e => e.Tag.Names.SortNames.English),
		_ => query.OrderBy(e => e.Tag.Names.SortNames.Romaji),
	};

	public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, string? target) where T : ITagLink
	{
		if (target == null) return query;

		return query.Where(t => t.Tag.NewTargets.Any(n => n == "all" || n == target || target.StartsWith(n + ':')));
	}
	
	public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, ArtistType a) where T : ITagLink
	{
		var entryType = ArtistHelper.IsVoiceSynthesizer(a) ? "voicesynthesizer" : "artist";
		return query.WhereTagHasTarget($"{entryType}:{a.ToString()}");
	}
	
	public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, DiscType d) where T : ITagLink
	{
		return query.WhereTagHasTarget($"album:{d.ToString()}");
	}
}
