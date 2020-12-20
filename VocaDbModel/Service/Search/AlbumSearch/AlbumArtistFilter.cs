#nullable disable

using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumArtistFilter : ISearchFilter<Album>
	{
		private readonly int _artistId;

		public AlbumArtistFilter(int artistId)
		{
			_artistId = artistId;
		}

		public QueryCost Cost => QueryCost.Medium;

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session)
		{
			return query.Where(a => a.AllArtists.Any(u => u.Artist.Id == _artistId));
		}

		public IQueryable<Album> Query(IDatabaseContext session)
		{
			return session.Query<ArtistForAlbum>()
				.Where(a => a.Artist.Id == _artistId)
				.Select(a => a.Album);
		}
	}
}
