using System.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class AlbumLinkQueryableExtender {

		public static IQueryable<T> WhereAlbumHasTag<T>(this IQueryable<T> query, string tagName)
			where T : IAlbumLink {
			
			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Album.Tags.Usages.Any(t => t.Tag.Names.SortNames.English == tagName || t.Tag.Names.SortNames.Romaji == tagName || t.Tag.Names.SortNames.Japanese == tagName));

		}

		public static IQueryable<T> WhereAlbumHasTag<T>(this IQueryable<T> query, int tagId)
			where T : IAlbumLink {

			if (tagId == 0)
				return query;

			return query.Where(s => s.Album.Tags.Usages.Any(t => t.Tag.Id == tagId));

		}

	}

}
