using System.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class AlbumLinkQueryableExtender {

		public static IQueryable<T> WhereAlbumHasTag<T>(this IQueryable<T> query, string tag)
			where T : IAlbumLink {
			
			if (string.IsNullOrEmpty(tag))
				return query;

			return query.Where(s => s.Album.Tags.Usages.Any(t => t.Tag.Name == tag));

		}

	}

}
