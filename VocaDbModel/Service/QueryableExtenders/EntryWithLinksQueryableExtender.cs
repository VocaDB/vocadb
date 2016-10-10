using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryWithLinksQueryableExtender {

		public static IQueryable<T> WhereHasLink<T, TLink>(this IQueryable<T> query, string url) where T : IEntryWithLinks<TLink> where TLink : WebLink {
			return query.Where(e => e.WebLinks.Any(l => l.Url == url));
		} 

	}
}
