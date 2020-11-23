using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumArtistFilter : ISearchFilter<Album>
	{
		private readonly int artistId;

		public AlbumArtistFilter(int artistId)
		{
			this.artistId = artistId;
		}

		public QueryCost Cost
		{
			get { return QueryCost.Medium; }
		}

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session)
		{
			return query.Where(a => a.AllArtists.Any(u => u.Artist.Id == artistId));
		}

		public IQueryable<Album> Query(IDatabaseContext session)
		{
			return session.Query<ArtistForAlbum>()
				.Where(a => a.Artist.Id == artistId)
				.Select(a => a.Album);
		}
	}
}
