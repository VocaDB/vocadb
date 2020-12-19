#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumNameFilter : ISearchFilter<Album>
	{
		private readonly string[] _names;

		public AlbumNameFilter(IEnumerable<string> names)
		{
			this._names = names.ToArray();
		}

		public QueryCost Cost => QueryCost.Medium;

		/*public void FilterResults(List<Album> albums, ISession session) {

			albums.RemoveAll(a => !(
				a.Names.Any(n => names.All(n2 => n.Value.IndexOf(n2, StringComparison.InvariantCultureIgnoreCase) != -1))));
		}

		public List<Album> GetResults(ISession session) {

			var q = session.Query<AlbumName>();

			foreach (var n2 in names)
				q = q.Where(n => n.Value.Contains(n2));

			return q.Select(n => n.Album).ToList();
		}*/

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session)
		{
			return query.WhereHasNameGeneric<Album, AlbumName>(SearchTextQuery.Create(_names, NameMatchMode.Words));
		}

		public IQueryable<Album> Query(IDatabaseContext session)
		{
			return session.Query<Album>().WhereHasName(new SearchTextQuery(string.Empty, NameMatchMode.Words, string.Empty, _names));
		}
	}
}
