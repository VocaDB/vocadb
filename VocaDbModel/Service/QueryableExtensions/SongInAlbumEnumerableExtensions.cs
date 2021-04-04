using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class SongInAlbumEnumerableExtensions
	{
		public static IEnumerable<SongInAlbum> WhereDiscNumberIs(this IEnumerable<SongInAlbum> query, int? discNumber)
		{
			if (!discNumber.HasValue)
				return query;
			return query.Where(s => s.DiscNumber == discNumber.Value);
		}
	}
}
