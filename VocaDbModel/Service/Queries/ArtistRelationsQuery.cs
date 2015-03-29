using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Queries {

	public class ArtistRelationsQuery {

		private readonly IRepositoryContext<Artist> ctx;
		private readonly ContentLanguagePreference languagePreference;

		private AlbumContract[] GetLatestAlbums(IRepositoryContext<Artist> session, Artist artist) {
			
			var id = artist.Id;

			var queryWithoutMain = session.OfType<ArtistForAlbum>().Query()
				.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport);

			var query = queryWithoutMain
				.WhereHasArtistParticipationStatus(artist, ArtistAlbumParticipationStatus.OnlyMainAlbums);

			var count = query.Count();

			query = count >= 4 ? query : queryWithoutMain;

			return query
				.Select(s => s.Album)
				.OrderByReleaseDate()
				.Take(6).ToArray()
				.Select(s => new AlbumContract(s, languagePreference))
				.ToArray();

		}

		private AlbumContract[] GetTopAlbums(IRepositoryContext<Artist> session, Artist artist, int[] latestAlbumIds) {
			
			var id = artist.Id;

			var queryWithoutMain = session.OfType<ArtistForAlbum>().Query()
				.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport 
					&& s.Album.RatingAverageInt > 0 && !latestAlbumIds.Contains(s.Album.Id));

			/*var query = queryWithoutMain
				.WhereHasArtistParticipationStatus(artist, ArtistAlbumParticipationStatus.OnlyMainAlbums);

			var count = query.Count();

			query = count >= 6 ? query : queryWithoutMain;*/

			return queryWithoutMain
				.Select(s => s.Album)
				.OrderByDescending(s => s.RatingAverageInt)
				.ThenByDescending(s => s.RatingCount)
				.Take(6).ToArray()
				.Select(s => new AlbumContract(s, languagePreference))
				.ToArray();

		}

		public ArtistRelationsQuery(IRepositoryContext<Artist> ctx, ContentLanguagePreference languagePreference) {
			this.ctx = ctx;
			this.languagePreference = languagePreference;
		}

		public ArtistRelationsForApi GetRelations(Artist artist, ArtistRelationsFields fields) {
			
			var contract = new ArtistRelationsForApi();

			if (fields.HasFlag(ArtistRelationsFields.LatestAlbums)) {
				contract.LatestAlbums = GetLatestAlbums(ctx, artist);				
			}

			if (fields.HasFlag(ArtistRelationsFields.PopularAlbums)) {
				var latestAlbumIds = contract.LatestAlbums != null ? contract.LatestAlbums.Select(a => a.Id).ToArray() : new int[0];				
				contract.PopularAlbums = GetTopAlbums(ctx, artist, latestAlbumIds);
			}

			if (fields.HasFlag(ArtistRelationsFields.LatestSongs)) {

				contract.LatestSongs = ctx.OfType<ArtistForSong>().Query()
					.Where(s => !s.Song.Deleted && s.Artist.Id == artist.Id && !s.IsSupport)
					.WhereIsMainSong(artist.ArtistType)
					.Select(s => s.Song)
					.OrderByDescending(s => s.CreateDate)
					.Take(8).ToArray()
					.Select(s => new SongContract(s, languagePreference))
					.ToArray();
				
			}

			if (fields.HasFlag(ArtistRelationsFields.PopularSongs)) {
				
				var latestSongIds = contract.LatestSongs != null ? contract.LatestSongs.Select(s => s.Id).ToArray() : new int[0];

				contract.PopularSongs = ctx.OfType<ArtistForSong>().Query()
					.Where(s => !s.Song.Deleted && s.Artist.Id == artist.Id && !s.IsSupport 
						&& s.Song.RatingScore > 0 && !latestSongIds.Contains(s.Song.Id))
					.Select(s => s.Song)
					.OrderByDescending(s => s.RatingScore)
					.Take(8).ToArray()
					.Select(s => new SongContract(s, languagePreference))
					.ToArray();

			}

			return contract;

		}

	}

}
