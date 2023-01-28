#nullable disable

using System.Runtime.Caching;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Queries;

public class ArtistRelationsQuery
{
	private static readonly SongOptionalFields s_songFields = SongOptionalFields.AdditionalNames | SongOptionalFields.MainPicture;
	private readonly ObjectCache _cache;
	private readonly IDatabaseContext _ctx;
	private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;
	private readonly ContentLanguagePreference _languagePreference;

	private AlbumForApiContract[] GetLatestAlbums(IDatabaseContext session, Artist artist)
	{
		var id = artist.Id;

		var queryWithoutMain = session.Query<ArtistForAlbum>()
			.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport);

		var query = queryWithoutMain
			.WhereHasArtistParticipationStatus(artist, ArtistAlbumParticipationStatus.OnlyMainAlbums);

		var count = query.Count();

		query = count >= 4 ? query : queryWithoutMain;

		return query
			.Select(s => s.Album)
			.OrderByReleaseDate(SortDirection.Descending)
			.Take(6).ToArray()
			.Select(s => new AlbumForApiContract(s, _languagePreference, _entryThumbPersister, AlbumOptionalFields.AdditionalNames | AlbumOptionalFields.MainPicture))
			.ToArray();
	}

	private ReleaseEventForApiContract[] GetLatestEvents(IDatabaseContext session, Artist artist)
	{
		var id = artist.Id;

		var cacheKey = $"{nameof(ArtistRelationsQuery)}.{nameof(GetLatestEvents)}.{id}.{_languagePreference}";

		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromHours(4)), () =>
		{
			return session.Query<ReleaseEvent>()
				.WhereNotDeleted()
				.Where(e => e.AllArtists.Any(a => a.Artist.Id == id))
				.WhereDateIsBetween(begin: null, end: DateTime.Today.AddMonths(6))
				.OrderByDate(SortDirection.Descending)
				.Take(3).ToArray()
				.Select(s => new ReleaseEventForApiContract(s, _languagePreference,
					ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series | ReleaseEventOptionalFields.Venue,
					_entryThumbPersister))
				.ToArray();
		});
	}

	private AlbumForApiContract[] GetTopAlbums(IDatabaseContext session, Artist artist, int[] latestAlbumIds)
	{
		var id = artist.Id;

		var queryWithoutMain = session.Query<ArtistForAlbum>()
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
			.Select(s => new AlbumForApiContract(s, _languagePreference, _entryThumbPersister, AlbumOptionalFields.AdditionalNames | AlbumOptionalFields.MainPicture))
			.ToArray();
	}

	private int[] GetLatestSongIds(IDatabaseContext ctx, Artist artist)
	{
		var cacheKey = $"ArtistRelationsQuery.GetLatestSongs.{artist.Id}";

		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromMinutes(5)), () =>
		{
			return ctx.Query<ArtistForSong>()
				.Where(s => !s.Song.Deleted && s.Artist.Id == artist.Id && !s.IsSupport)
				.WhereIsMainSong(artist.ArtistType)
				.OrderByPublishDate(SortDirection.Descending)
				.Select(s => s.Song.Id)
				.Take(8)
				.ToArray();
		}, allowCaching: ids => ids.Length >= 8);
	}

	private SongForApiContract[] GetLatestSongs(IDatabaseContext ctx, Artist artist)
	{
		return ctx
			.LoadMultiple<Song>(GetLatestSongIds(ctx, artist))
			.OrderByPublishDate(SortDirection.Descending)
			.ToArray()
			.Select(s => new SongForApiContract(s, _languagePreference, s_songFields))
			.ToArray();
	}

	private int[] GetTopSongIds(IDatabaseContext ctx, Artist artist, SongForApiContract[] latestSongs)
	{
		var cacheKey = $"ArtistRelationsQuery.GetTopSongs.{artist.Id}";

		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(1), () =>
		{
			var latestSongIds = latestSongs != null ? latestSongs.Select(s => s.Id).ToArray() : Array.Empty<int>();

			return ctx.Query<ArtistForSong>()
				.Where(s => !s.Song.Deleted
					&& s.Artist.Id == artist.Id
					&& !s.IsSupport
					&& s.Roles != ArtistRoles.VocalDataProvider
					&& s.Song.RatingScore > 0
					&& !latestSongIds.Contains(s.Song.Id))
				.OrderBy(SongSortRule.RatingScore, _languagePreference)
				.Select(s => s.Song.Id)
				.Take(8)
				.ToArray();
		});
	}

	private SongForApiContract[] GetTopSongs(IDatabaseContext ctx, Artist artist, SongForApiContract[] latestSongs)
	{
		return ctx.LoadMultiple<Song>(GetTopSongIds(ctx, artist, latestSongs))
			.OrderByDescending(s => s.RatingScore)
			.ToArray()
			.Select(s => new SongForApiContract(s, _languagePreference, s_songFields))
			.ToArray();
	}

	public ArtistRelationsQuery(IDatabaseContext ctx, ContentLanguagePreference languagePreference, ObjectCache cache, IAggregatedEntryImageUrlFactory entryThumbPersister)
	{
		_ctx = ctx;
		_languagePreference = languagePreference;
		_cache = cache;
		_entryThumbPersister = entryThumbPersister;
	}

	public ArtistRelationsForApi GetRelations(Artist artist, ArtistRelationsFields fields)
	{
		var contract = new ArtistRelationsForApi();

		if (fields.HasFlag(ArtistRelationsFields.LatestAlbums))
		{
			contract.LatestAlbums = GetLatestAlbums(_ctx, artist);
		}

		if (fields.HasFlag(ArtistRelationsFields.PopularAlbums))
		{
			var latestAlbumIds = contract.LatestAlbums != null ? contract.LatestAlbums.Select(a => a.Id).ToArray() : Array.Empty<int>();
			contract.PopularAlbums = GetTopAlbums(_ctx, artist, latestAlbumIds);
		}

		if (fields.HasFlag(ArtistRelationsFields.LatestSongs))
		{
			contract.LatestSongs = GetLatestSongs(_ctx, artist);
		}

		if (fields.HasFlag(ArtistRelationsFields.PopularSongs))
		{
			contract.PopularSongs = GetTopSongs(_ctx, artist, contract.LatestSongs);
		}

		if (fields.HasFlag(ArtistRelationsFields.LatestEvents))
		{
			contract.LatestEvents = GetLatestEvents(_ctx, artist);
		}

		return contract;
	}

	public TopStatContract<TranslatedArtistContract>[] GetTopVoicebanks(Artist artist)
	{
		var types = ArtistHelper.VoiceSynthesizerTypes;
		var roles = ArtistRoles.Arranger | ArtistRoles.Composer | ArtistRoles.Other | ArtistRoles.VoiceManipulator;

		var topVocaloidIdsAndCounts = _ctx
			.Query<ArtistForSong>()
			.Where(a => types.Contains(a.Artist.ArtistType) && !a.Song.Deleted
				&& a.Song.AllArtists.Any(ar => !ar.IsSupport && ar.Artist.Id == artist.Id && (ar.Roles == ArtistRoles.Default || (ar.Roles & roles) != ArtistRoles.Default)))
			.GroupBy(a => a.Artist.Id)
			.Select(a => new
			{
				ArtistId = a.Key,
				Count = a.Count()
			})
			.OrderByDescending(a => a.Count)
			.Take(3)
			.ToDictionary(a => a.ArtistId, a => a.Count);

		var topVocaloidIds = topVocaloidIdsAndCounts.Select(i => i.Key).ToArray();

		var topVocaloids = _ctx.Query<Artist>()
			.Where(a => topVocaloidIds.Contains(a.Id))
			.ToArray()
			.Select(a => new TopStatContract<TranslatedArtistContract>
			{
				Data = new TranslatedArtistContract(a),
				Count = topVocaloidIdsAndCounts[a.Id]
			})
			.OrderByDescending(d => d.Count)
			.ToArray();

		return topVocaloids;
	}
}
