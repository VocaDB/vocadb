#nullable disable

using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Service.QueryableExtensions
{
	/// <summary>
	/// Extension methods for <see cref="IQueryable{IEntryWithLinks{TLink}}"/>.
	/// Also see <see cref="WebLinkQueryableExtensions"/>.
	/// </summary>
	public static class EntryWithLinksQueryableExtensions
	{
#nullable enable
		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url)
			where T : IEntryWithLinks<TLink> where TLink : WebLink
		{
			return WhereHasLink<T, TLink>(query, url, NameMatchMode.Exact);
		}
#nullable disable

		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url, WebLinkVariationTypes variationTypes)
			where T : IEntryWithLinks<TLink> where TLink : WebLink
		{
			var variations = WebLinkVariationsFactory.GetWebLinkVariations(url, variationTypes);
			return query.Where(e => e.WebLinks.Any(w => variations.Contains(w.Url)));
		}

#nullable enable
		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url, NameMatchMode matchMode)
			where T : IEntryWithLinks<TLink> where TLink : WebLink => matchMode switch
		{
			NameMatchMode.StartsWith => query.Where(e => e.WebLinks.Any(l => l.Url.StartsWith(url))),
			NameMatchMode.Partial or NameMatchMode.Words => query.Where(e => e.WebLinks.Any(l => l.Url.Contains(url))),
			_ => query.Where(e => e.WebLinks.Any(l => l.Url == url)),
		};
#nullable disable
	}
}
