#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumArtistNameFilter : ISearchFilter<Album>
	{
		private readonly string[] _artistNames;

		public AlbumArtistNameFilter(IEnumerable<string> artistNames)
		{
			this._artistNames = artistNames.ToArray();
		}

		public QueryCost Cost => QueryCost.High;

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session)
		{
			return query.Where(a => a.AllArtists.Any(u => u.Artist.Names.Names.Any(a2 => _artistNames.Any(na => a2.Value.Contains(na)))));
		}

		public IQueryable<Album> Query(IDatabaseContext session)
		{
			/*var n = artistNames.First();
			return session.Query<ArtistName>()
				.Where(an => an.Value.Contains(n))
				.SelectMany(an => an.Artist.AllAlbums)
				.Select(an => an.Album)
				.Distinct();*/

			/*return session.Query<ArtistName>()
				.Where(an => artistNames.Any(na => an.Value.Contains(na)))
				.SelectMany(an => an.Artist.AllAlbums)
				.Select(an => an.Album)
				.Distinct();				*/

			if (_artistNames.Length == 2)
			{
				var n1 = _artistNames.ElementAt(0);
				var n2 = _artistNames.ElementAt(1);
				return session.Query<Album>().Where(a => a.AllArtists.Any(u => u.Artist.Names.Names.Any(an => an.Value.Contains(n1)))
					&& a.AllArtists.Any(u => u.Artist.Names.Names.Any(an => an.Value.Contains(n2))));
			}

			return session.Query<Album>();
			//return session.Query<Album>().Where(a => artistNames.All(na => a.AllArtists.Any(u => u.Artist.Names.Names.Any(a2 => a2.Value.Contains(na)))));
		}
	}
}
