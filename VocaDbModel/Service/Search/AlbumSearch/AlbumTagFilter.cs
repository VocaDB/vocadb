using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumTagFilter : ISearchFilter<Album> {

		private readonly string tagName;

		public AlbumTagFilter(string tagName) {
			this.tagName = tagName;
		}

		public QueryCost Cost {
			get { return QueryCost.Medium; }
		}

		public IQueryable<Album> Filter(IQueryable<Album> query, IRepositoryContext session) {

			return query.Where(a => a.Tags.Usages.Any(u => u.Tag.Name == tagName));

		}

		public void FilterResults(List<Album> albums, IRepositoryContext session) {

			albums.RemoveAll(a => !(a.Tags.HasTag(tagName)));

		}

		public List<Album> GetResults(IRepositoryContext session) {

			return session.Query<AlbumTagUsage>()
				.Where(a => a.Tag.Name == tagName)
				.Select(a => a.Album)
				.ToList();

		}

		public IQueryable<Album> Query(IRepositoryContext session) {

			return session.Query<AlbumTagUsage>()
				.Where(a => a.Tag.Name == tagName)
				.Select(a => a.Album);

		}

	}

}
