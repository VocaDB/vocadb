#nullable disable

using System.Runtime.Caching;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service;

public class OtherService : ServiceBase
{
	class EntryComparer : IEqualityComparer<IEntryWithNames>
	{
#nullable enable
		public bool Equals(IEntryWithNames? x, IEntryWithNames? y)
		{
			return x.EntryType == y.EntryType && x.Id == y.Id;
		}

		public int GetHashCode(IEntryWithNames obj)
		{
			return obj.Id;
		}
#nullable disable
	}

	private readonly ObjectCache _cache;
	private readonly IUserIconFactory _userIconFactory;
	private readonly EntryForApiContractFactory _entryForApiContractFactory;
	private readonly IAggregatedEntryImageUrlFactory _thumbPersister;

	public AlbumForApiContract[] GetTopAlbums(ContentLanguagePreference languagePreference, AlbumOptionalFields fields, int[] ignoreIds)
	{
		return HandleQuery(session => GetTopAlbums(session, ignoreIds, languagePreference, fields));
	}

	private AlbumForApiContract[] GetTopAlbums(ISession session, int[] recentIds, ContentLanguagePreference languagePreference, AlbumOptionalFields fields)
	{
		var minRatings = 2; // Minimum number of ratings
		var sampleSize = 300; // Get this many popular albums to be rotated when cache expires
		var albumCount = 7; // This many albums are shown, the albums are rotated when cache expires

		// If only a small number of rated albums, reduce minimum ratings count
		var totalRatedAlbumCount = session.Query<Album>().Count(a => !a.Deleted && a.RatingCount >= minRatings && a.RatingAverageInt >= 300);
		if (totalRatedAlbumCount < albumCount)
		{
			minRatings = 1;
		}

		// Find Ids of albums that match the popularity filters, take maximum of sampleSize albums
		var popularIds = session.Query<Album>()
			.WhereHasArtist(AppConfig.FilteredArtistId)
			.Where(a => !a.Deleted
				&& a.RatingCount >= minRatings && a.RatingAverageInt >= 300 // Filter by number of ratings and average rating
				&& !recentIds.Contains(a.Id))                       // Filter out recent albums (that are already shown)
			.OrderByDescending(a => a.RatingTotal)
			.Select(a => a.Id)
			.Take(sampleSize)
			.ToArray();

		// Pick random albums to be displayed from the group of popular albums
		var randomIds = CollectionHelper
			.GetRandomItems(popularIds, albumCount)
			.ToArray();

		var random = session.Query<Album>()
			.Where(a => randomIds.Contains(a.Id))
			.OrderByDescending(a => a.RatingAverageInt)
			.ToArray();

		var popularAlbumContracts = random
			.Select(a => new AlbumForApiContract(a, null, languagePreference, PermissionContext, _thumbPersister, fields, SongOptionalFields.None))
			.ToArray();

		return popularAlbumContracts;
	}

	private AlbumForApiContract[] GetTopAlbumsCached(ISession session, int[] recentIds, ContentLanguagePreference languagePreference, AlbumOptionalFields fields)
	{
		var cacheKey = $"OtherService.PopularAlbums.{languagePreference}";
		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromHours(24)), () => GetTopAlbums(session, recentIds, languagePreference, fields));
	}

	public AlbumForApiContract[] GetRecentAlbums(ContentLanguagePreference languagePreference, AlbumOptionalFields fields)
	{
		return HandleQuery(session => GetRecentAlbums(session, languagePreference, fields));
	}

	private AlbumForApiContract[] GetRecentAlbums(ISession session, ContentLanguagePreference languagePreference, AlbumOptionalFields fields)
	{
		var cacheKey = $"OtherService.RecentAlbums.{languagePreference}";
		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromHours(1)), () =>
		{
			var now = DateTime.Now;

			var upcoming = session.Query<Album>()
				.WhereHasArtist(AppConfig.FilteredArtistId)
				.Where(a => !a.Deleted)
				.WhereHasReleaseDate()
				.WhereReleaseDateIsAfter(now)
				.OrderByReleaseDate(SortDirection.Ascending)
				.Take(4)
				.ToArray();

			var recent = session.Query<Album>()
				.WhereHasArtist(AppConfig.FilteredArtistId)
				.Where(a => !a.Deleted)
				.WhereHasReleaseDate()
				.WhereReleaseDateIsBefore(now)
				.OrderByReleaseDate(SortDirection.Descending)
				.Take(3)
				.ToArray();

			var newAlbumContracts = upcoming.Reverse().Concat(recent)
				.Select(a => new AlbumForApiContract(a, null, languagePreference, PermissionContext, _thumbPersister, fields, SongOptionalFields.None))
				.ToArray();

			return newAlbumContracts;
		});
	}

	private ReleaseEventForApiContract[] GetRecentEvents(ISession session)
	{
		var count = 6;
		var cacheKey = $"OtherService.RecentEvents.{LanguagePreference}";
		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(24), () =>
		{
			var minDate = DateTime.Now - TimeSpan.FromDays(2);
			var maxDate = DateTime.Now + TimeSpan.FromDays(14);

			var recentEvents = session.Query<ReleaseEvent>()
				.WhereNotDeleted()
				.FromDateOnwards(minDate)
				.OrderByDate(SortDirection.Ascending)
				.Take(count)
				.ToArray();

			var entryContracts = recentEvents.Select(i => new ReleaseEventForApiContract(
				i,
				LanguagePreference,
				PermissionContext,
				ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series | ReleaseEventOptionalFields.Venue,
				_thumbPersister
			));

			return entryContracts.ToArray();
		});
	}

	public async Task<SongForApiContract[]> GetHighlightedSongs(ContentLanguagePreference languagePreference, SongOptionalFields fields)
	{
		return await HandleQueryAsync(async session =>
		{
			return (await GetHighlightedSongs(session))
				.Select(s => new SongForApiContract(s, languagePreference, PermissionContext, fields))
				.ToArray();
		});
	}

	private async Task<Song[]> GetHighlightedSongs(ISession session)
	{
		var cacheKey = "OtherService.HighlightedSongs";
		var cachedSongIds = (int[])_cache.Get(cacheKey);

		if (cachedSongIds != null)
		{
			var cachedSongs = await session.Query<Song>()
				.WhereIdIn(cachedSongIds)
				.WhereHasPV()
				.ToListAsync();
			return cachedSongs.OrderByIds(cachedSongIds).WhereNotNull().ToArray();
		}

		var cutoffDate = DateTime.Now - TimeSpan.FromDays(2);
		var maxSongs = 1000;
		var songCount = 20;

		// Load at most maxSongs songs for cutoff date
		var recentSongIdAndScore =
			session.Query<Song>()
			.WhereHasArtist(AppConfig.FilteredArtistId)
			.WhereNotDeleted()
			.WhereHasPV()
			.Where(s => s.CreateDate >= cutoffDate)
			.OrderByDescending(s => s.CreateDate)
			.Take(maxSongs)
			.Select(s => new
			{
				s.Id,
				s.RatingScore
			})
			.ToArray();

		// Get song Ids
		var songIds = recentSongIdAndScore
			.OrderByDescending(s => s.RatingScore)
			.Take(songCount)
			.Select(s => s.Id)
			.ToArray();

		// Load the songs
		var recentSongs = session.Query<Song>()
			.Where(s => songIds.Contains(s.Id))
			.OrderBy(SongSortRule.RatingScore)
			.ToArray();

		Song[] songs;

		// If there's enough songs for cutoff date, return them, otherwise load more songs.
		if (recentSongs.Length >= songCount)
		{
			songs = recentSongs;
		}
		else
		{
			var moreSongs =
				session.Query<Song>()
				.WhereHasArtist(AppConfig.FilteredArtistId)
				.WhereNotDeleted()
				.WhereHasPV()
				.Where(s => s.CreateDate < cutoffDate)
				.OrderByDescending(s => s.CreateDate)
				.Take(songCount - recentSongs.Length)
				.ToArray();

			songs =
				recentSongs
				.Concat(moreSongs)
				.OrderByDescending(s => s.RatingScore)
				.ToArray();
		}

		var allSongIds = songs.Select(s => s.Id).ToArray();
		_cache.Add(cacheKey, allSongIds, DateTime.Now + TimeSpan.FromMinutes(15));

		return songs;
	}

	private IEnumerable<EntryWithCommentsContract> CreateEntryWithCommentsContract<T>(IEnumerable<T> comments, Func<T, EntryForApiContract> entryContractFac)
		where T : Comment
	{
		return comments.GroupBy(e => e.Entry, new EntryComparer()).Select(e => new EntryWithCommentsContract(entryContractFac(e.First()), e.Select(c => new CommentForApiContract(c, _userIconFactory)).ToArray()));
	}

	private async Task<EntryWithCommentsContract[]> GetRecentCommentsAsync(ISession session, int maxComments)
	{
		// FIXME: this returns less comments if there are deleted entries.
		// See also: https://github.com/VocaDB/vocadb/pull/663#pullrequestreview-545596680
		var comments = (await session.Query<Comment>()
			.WhereNotDeleted()
			.Where(c => !(c is UserComment))
			.OrderByDescending(c => c.Created)
			.Take(maxComments)
			.ToListAsync())
			.Where(c => !c.Entry.Deleted);

		var combined = comments
			.OrderByDescending(c => c.Created)
			.Take(maxComments);

		var contracts = CreateEntryWithCommentsContract(
			combined,
			c => _entryForApiContractFactory.Create(
				c.Entry,
				EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture,
				LanguagePreference
			)
		).ToArray();

		return contracts;
	}

	public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory,
		IUserIconFactory userIconFactory, EntryForApiContractFactory entryForApiContractFactory, ObjectCache cache, IAggregatedEntryImageUrlFactory thumbPersister)
		: base(sessionFactory, permissionContext, entryLinkFactory)
	{
		_userIconFactory = userIconFactory;
		_entryForApiContractFactory = entryForApiContractFactory;
		_cache = cache;
		_thumbPersister = thumbPersister;
	}

	public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified)
	{
		HandleTransaction(session => AuditLog(doingWhat, session, who, category));
	}

	public string[] FindNames(SearchTextQuery textQuery, int maxResults)
	{
		if (textQuery.IsEmpty)
			return Array.Empty<string>();

		var artistTextQuery = ArtistSearchTextQuery.Create(textQuery);
		var tagTextQuery = TagSearchTextQuery.Create(textQuery);

		return HandleQuery(session =>
		{
			var artistNames = session.Query<ArtistName>()
				.WhereArtistNameIs(artistTextQuery)
				.Where(a => !a.Artist.Deleted)
				.Select(n => n.Value)
				.OrderBy(n => n)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			var albumNames = session.Query<AlbumName>()
				.WhereEntryNameIs(textQuery)
				.Where(a => !a.Album.Deleted)
				.Select(n => n.Value)
				.OrderBy(n => n)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			var songNames = session.Query<SongName>()
				.WhereEntryNameIs(textQuery)
				.Where(a => !a.Song.Deleted)
				.Select(n => n.Value)
				.OrderBy(n => n)
				.Distinct()
				.Take(maxResults)
				.ToArray();

			var eventNames = session.Query<EventName>()
				.WhereEntryNameIs(textQuery)
				.Where(a => !a.Entry.Deleted)
				.Select(t => t.Value)
				.OrderBy(t => t)
				.Take(maxResults)
				.ToArray();

			var tagNames = session.Query<TagName>()
				.WhereEntryNameIs(tagTextQuery)
				.Where(a => !a.Entry.Deleted)
				.Select(t => t.Value)
				.OrderBy(t => t)
				.Take(maxResults)
				.ToArray();

			var allNames = artistNames
				.Concat(albumNames)
				.Concat(songNames)
				.Concat(eventNames)
				.Concat(tagNames)
				.Distinct()
				.OrderBy(n => n)
				.Take(maxResults)
				.ToArray();

			return NameHelper.MoveExactNamesToTop(allNames, textQuery.Query);
		});
	}

	public async Task<FrontPageForApiContract> GetFrontPageForApiContent()
	{
		const int maxFetchActivityEntries = 60;
		const int maxDisplayedActivityEntries = 15;

		return await HandleQueryAsync(async session =>
		{
			// FIXME: this returns less comments if there are deleted entries.
			// See also: https://github.com/VocaDB/vocadb/pull/663#pullrequestreview-545596680
			var activityEntries = (await session.Query<ActivityEntry>()
				.OrderByDescending(a => a.CreateDate)
				.Take(maxFetchActivityEntries)
				.ToListAsync())
				.Where(a => !a.EntryBase.Deleted)
				.DistinctBy(a => new { EID = a.EntryBase.Id, AID = a.Author.Id })
				.Take(maxDisplayedActivityEntries);

			var newAlbums = GetRecentAlbums(
				session: session,
				languagePreference: LanguagePreference,
				fields: AlbumOptionalFields.MainPicture
			);

			var recentIds = newAlbums.Select(a => a.Id).ToArray();
			var topAlbums = GetTopAlbumsCached(
				session: session,
				recentIds: recentIds,
				languagePreference: LanguagePreference,
				fields: AlbumOptionalFields.MainPicture
			);

			var newSongs = await GetHighlightedSongs(session);

			var firstSongVote = newSongs.Any()
				? await session.Query<FavoriteSongForUser>().FirstOrDefaultAsync(s => s.Song.Id == newSongs.First().Id && s.User.Id == PermissionContext.LoggedUserId)
				: null;

			var recentComments = await GetRecentCommentsAsync(session, 9);

			var recentEvents = GetRecentEvents(session);

			return new FrontPageForApiContract(
				activityEntries: activityEntries,
				newAlbums: newAlbums,
				newEvents: recentEvents,
				recentComments: recentComments,
				topAlbums: topAlbums,
				newSongs: newSongs,
				firstSongRating: firstSongVote is not null
					? firstSongVote.Rating
					: SongVoteRating.Nothing,
				languagePreference: PermissionContext.LanguagePreference,
				userIconFactory: _userIconFactory,
				permissionContext: PermissionContext,
				entryForApiContractFactory: _entryForApiContractFactory
			);
		});
	}

	public ArtistForApiContract[] GetHighlightedArtistsCached(ContentLanguagePreference lang = ContentLanguagePreference.Default)
	{
		var cacheKey = $"OtherService.HighlightedArtists.{lang}";
		return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(TimeSpan.FromHours(12)), () => GetHighlightedArtists(lang));
	}

	private ArtistForApiContract[] GetHighlightedArtists(ContentLanguagePreference languagePreference)
	{
		const int artistCount = 6;

		return HandleQuery(session =>
		{
			// TODO: Imeplemnt daily rotation
			var artists = session.Query<Artist>()
				.Where(a => a.ArtistType == ArtistType.Producer)
				.Where(a => a.Tags.Usages.Any(usage => usage.Tag.CategoryName == "Genres"))
				.Where(a => a.Users.Count > 0)
				.OrderBy(a => a.AllSongs.Select(s => s.Song.RatingScore).Sum() / a.Users.Count)
				.Take(artistCount)
				.Select(a => new ArtistForApiContract(a, languagePreference, PermissionContext, _thumbPersister, ArtistOptionalFields.None))
				.ToArray();

			return artists;
		});
	}

	public IPRule[] GetIPRules() => HandleQuery(session => session.Query<IPRule>().ToArray());

	public Task<EntryWithCommentsContract[]> GetRecentComments() => HandleQueryAsync(session => GetRecentCommentsAsync(session, 50));
}
