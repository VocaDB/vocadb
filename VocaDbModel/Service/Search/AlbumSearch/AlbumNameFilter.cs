using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumNameFilter : ISearchFilter<Album> {

		private readonly string[] names;

		public AlbumNameFilter(IEnumerable<string> names) {
			this.names = names.ToArray();
		}

		public QueryCost Cost {
			get { return QueryCost.Medium; }
		}

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

		public IQueryable<Album> Filter(IQueryable<Album> query, IQuerySource session) {

			return query.SelectMany(a => FindHelpers.AddEntryNameFilter(a.Names.Names.AsQueryable(), new SearchTextQuery(string.Empty, NameMatchMode.Words, string.Empty, names))).Select(n => n.Album);

		}

		public IQueryable<Album> Query(IQuerySource session) {

			var q = session.Query<AlbumName>();

			return FindHelpers.AddEntryNameFilter(q, new SearchTextQuery(string.Empty, NameMatchMode.Words, string.Empty, names))
				.Select(n => n.Album);

		}

	}

}
