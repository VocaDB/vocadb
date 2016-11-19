using System;
using System.Linq;
using System.Runtime.Caching;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Queries {

	public class ArtistRelationsQuery {

		private static readonly SongOptionalFields songFields = SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl;
		private readonly ObjectCache cache;
		private readonly IDatabaseContext<Artist> ctx;
		private readonly ContentLanguagePreference languagePreference;

		private AlbumContract[] GetLatestAlbums(IDatabaseContext<Artist> session, Artist artist) {
			
			var id = artist.Id;

			var queryWithoutMain = session.OfType<ArtistForAlbum>().Query()
				.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport);

			var query = queryWithoutMain
				.WhereHasArtistParticipationStatus(artist, ArtistAlbumParticipationStatus.OnlyMainAlbums);

			var count = query.Count();

			query = count >= 4 ? query : queryWithoutMain;

			return query
				.Select(s => s.Album)
				.OrderByReleaseDate(SortDirection.Descending)
				.Take(6).ToArray()
				.Select(s => new AlbumContract(s, languagePreference))
				.ToArray();

		}

		private AlbumContract[] GetTopAlbums(IDatabaseContext<Artist> session, Artist artist, int[] latestAlbumIds) {
			
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

		private int[] GetLatestSongIds(IDatabaseContext ctx, Artist artist) {

			var cacheKey = string.Format("ArtistRelationsQuery.GetLatestSongs.{0}", artist.Id);

			return cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromMinutes(15)), () => {

				return ctx.OfType<ArtistForSong>().Query()
					.Where(s => !s.Song.Deleted && s.Artist.Id == artist.Id && !s.IsSupport)
					.WhereIsMainSong(artist.ArtistType)
					.OrderByPublishDate(SortDirection.Descending)
					.Select(s => s.Song.Id)
					.Take(8)
					.ToArray();

			});

		}

		private SongForApiContract[] GetLatestSongs(IDatabaseContext ctx, Artist artist) {

			return ctx
				.LoadMultiple<Song>(GetLatestSongIds(ctx, artist))
				.OrderByPublishDate(SortDirection.Descending)
				.ToArray()
				.Select(s => new SongForApiContract(s, languagePreference, songFields))
				.ToArray();

		}

		private int[] GetTopSongIds(IDatabaseContext ctx, Artist artist, SongForApiContract[] latestSongs) {

			var cacheKey = string.Format("ArtistRelationsQuery.GetTopSongs.{0}", artist.Id);

			return cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(1), () => {

				var latestSongIds = latestSongs != null ? latestSongs.Select(s => s.Id).ToArray() : new int[0];

				return ctx.OfType<ArtistForSong>().Query()
					.Where(s => !s.Song.Deleted && s.Artist.Id == artist.Id && !s.IsSupport
						&& s.Song.RatingScore > 0 && !latestSongIds.Contains(s.Song.Id))
					.OrderBy(SongSortRule.RatingScore, languagePreference)
					.Select(s => s.Song.Id)
					.Take(8)
					.ToArray();

			});

		}

		private SongForApiContract[] GetTopSongs(IDatabaseContext ctx, Artist artist, SongForApiContract[] latestSongs) {

			return ctx.LoadMultiple<Song>(GetTopSongIds(ctx, artist, latestSongs))
				.OrderByDescending(s => s.RatingScore)
				.ToArray()
				.Select(s => new SongForApiContract(s, languagePreference, songFields))
				.ToArray();

		}

		public ArtistRelationsQuery(IDatabaseContext<Artist> ctx, ContentLanguagePreference languagePreference, ObjectCache cache) {
			this.ctx = ctx;
			this.languagePreference = languagePreference;
			this.cache = cache;
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
				contract.LatestSongs = GetLatestSongs(ctx, artist);				
			}

			if (fields.HasFlag(ArtistRelationsFields.PopularSongs)) {
				contract.PopularSongs = GetTopSongs(ctx, artist, contract.LatestSongs);
			}

			return contract;

		}

	}

}
