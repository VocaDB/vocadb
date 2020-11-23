using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Service.QueryableExtenders
{
	/// <summary>
	/// Extension methods for <see cref="IQueryable{IEntryWithLinks{TLink}}"/>.
	/// Also see <see cref="WebLinkQueryableExtender"/>.
	/// </summary>
	public static class EntryWithLinksQueryableExtender
	{
		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url)
			where T : IEntryWithLinks<TLink> where TLink : WebLink
		{
			return WhereHasLink<T, TLink>(query, url, NameMatchMode.Exact);
		}

		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url, WebLinkVariationTypes variationTypes)
			where T : IEntryWithLinks<TLink> where TLink : WebLink
		{
			var variations = WebLinkVariationsFactory.GetWebLinkVariations(url, variationTypes);
			return query.Where(e => e.WebLinks.Any(w => variations.Contains(w.Url)));
		}

		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url, NameMatchMode matchMode)
			where T : IEntryWithLinks<TLink> where TLink : WebLink
		{
			switch (matchMode)
			{
				case NameMatchMode.StartsWith:
					return query.Where(e => e.WebLinks.Any(l => l.Url.StartsWith(url)));
				case NameMatchMode.Partial:
				case NameMatchMode.Words:
					return query.Where(e => e.WebLinks.Any(l => l.Url.Contains(url)));
				default:
					return query.Where(e => e.WebLinks.Any(l => l.Url == url));
			}
		}
	}
}
