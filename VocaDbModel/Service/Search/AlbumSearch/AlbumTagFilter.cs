using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumTagFilter : ISearchFilter<Album> {

		private readonly string tagName;

		public AlbumTagFilter(string tagName) {
			this.tagName = tagName;
		}

		public QueryCost Cost {
			get { return QueryCost.Medium; }
		}

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session) {

			return query.Where(a => a.Tags.Usages.Any(u => u.Tag.Name == tagName));

		}

		public void FilterResults(List<Album> albums, IDatabaseContext session) {

			albums.RemoveAll(a => !(a.Tags.HasTag(tagName)));

		}

		public List<Album> GetResults(IDatabaseContext session) {

			return session.Query<AlbumTagUsage>()
				.Where(a => a.Tag.Name == tagName)
				.Select(a => a.Album)
				.ToList();

		}

		public IQueryable<Album> Query(IDatabaseContext session) {

			return session.Query<AlbumTagUsage>()
				.Where(a => a.Tag.Name == tagName)
				.Select(a => a.Album);

		}

	}

}
