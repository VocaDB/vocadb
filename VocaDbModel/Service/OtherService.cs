using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		class EntryComparer : IEqualityComparer<IEntryWithNames> {

			public bool Equals(IEntryWithNames x, IEntryWithNames y) {
				return x.EntryType == y.EntryType && x.Id == y.Id;
			}

			public int GetHashCode(IEntryWithNames obj) {
				return obj.Id;
			}

		}

		private readonly ObjectCache cache;
		private readonly IUserIconFactory userIconFactory;
		private readonly EntryForApiContractFactory entryForApiContractFactory;
		private readonly IEntryThumbPersister thumbPersister;

		public AlbumForApiContract[] GetTopAlbums(ContentLanguagePreference languagePreference, AlbumOptionalFields fields, int[] ignoreIds) {
			return HandleQuery(session => GetTopAlbums(session, ignoreIds, languagePreference, fields));
		}

		private AlbumForApiContract[] GetTopAlbums(ISession session, int[] recentIds, ContentLanguagePreference languagePreference, AlbumOptionalFields fields) {

			var minRatings = 2; // Minimum number of ratings
			var sampleSize = 300; // Get this many popular albums to be rotated when cache expires
			var albumCount = 7; // This many albums are shown, the albums are rotated when cache expires
			var cacheKey = "OtherService.PopularAlbums." + languagePreference;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumForApiContract(a, LanguagePreference, thumbPersister, fields)).ToArray();

			// If only a small number of rated albums, reduce minimum ratings count
			var totalRatedAlbumCount = session.Query<Album>().Count(a => !a.Deleted && a.RatingCount >= minRatings && a.RatingAverageInt >= 300);
			if (totalRatedAlbumCount < albumCount) {
				minRatings = 1;
			}

			// Find Ids of albums that match the popularity filters, take maximum of sampleSize albums
			var popularIds = session.Query<Album>()
				.Where(a => !a.Deleted 
					&& a.RatingCount >= minRatings && a.RatingAverageInt >= 300	// Filter by number of ratings and average rating
					&& !recentIds.Contains(a.Id))						// Filter out recent albums (that are already shown)
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

			var popularAlbumsCached = random
				.Select(a => new TranslatedAlbumContract(a))
				.ToArray();

			var popularAlbumContracts = random
				.Select(a => new AlbumForApiContract(a, null, languagePreference, thumbPersister, true, fields, SongOptionalFields.None))
				.ToArray();

			cache.Add(cacheKey, popularAlbumsCached, DateTime.Now + TimeSpan.FromHours(24));

			return popularAlbumContracts;

		}

		public AlbumForApiContract[] GetRecentAlbums(ContentLanguagePreference languagePreference, AlbumOptionalFields fields) {
			return HandleQuery(session => GetRecentAlbums(session, languagePreference, fields));
		}

		private AlbumForApiContract[] GetRecentAlbums(ISession session, ContentLanguagePreference languagePreference, AlbumOptionalFields fields) {

			var cacheKey = "OtherService.RecentAlbums." + languagePreference;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumForApiContract(a, LanguagePreference, thumbPersister, fields)).ToArray();

			var now = DateTime.Now;

			var upcoming = session.Query<Album>()
				.Where(a => !a.Deleted)
				.WhereHasReleaseDate()
				.WhereReleaseDateIsAfter(now)
				.OrderByReleaseDate(SortDirection.Ascending)
				.Take(4)
				.ToArray();

			var recent = session.Query<Album>()
				.Where(a => !a.Deleted)
				.WhereHasReleaseDate()
				.WhereReleaseDateIsBefore(now)
				.OrderByReleaseDate(SortDirection.Descending)
				.Take(3)
				.ToArray();

			var newAlbums = upcoming.Reverse().Concat(recent)
				.Select(a => new TranslatedAlbumContract(a))
				.ToArray();

			var newAlbumContracts = upcoming.Reverse().Concat(recent)
				.Select(a => new AlbumForApiContract(a, null, languagePreference, thumbPersister, true, fields, SongOptionalFields.None))
				.ToArray();

			cache.Add(cacheKey, newAlbums, DateTime.Now + TimeSpan.FromHours(1));

			return newAlbumContracts;

		}

		private ReleaseEventForApiContract[] GetRecentEvents(ISession session, bool ssl) {

			var count = 3;
			var cacheKey = string.Format("OtherService.RecentEvents.{0}.{1}", LanguagePreference, ssl);
			return cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(24), () => {

				var minDate = DateTime.Now - TimeSpan.FromDays(2);
				var maxDate = DateTime.Now + TimeSpan.FromDays(14);

				var recentEvents = session.Query<ReleaseEvent>()
					.WhereNotDeleted()
					.WhereDateIsBetween(minDate, maxDate)
					.OrderByDate(SortDirection.Ascending)
					.Take(count)
					.ToArray();

				var entryContracts = recentEvents.Select(i => 
					new ReleaseEventForApiContract(i, LanguagePreference, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series, 
					thumbPersister, ssl));

				return entryContracts.ToArray();

			});

		}

		private EntryWithCommentsContract[] GetRecentComments(ISession session, bool ssl) {
			
			var cacheKey = string.Format("OtherService.RecentComments.{0}.{1}", LanguagePreference, ssl);
			var item = (EntryWithCommentsContract[])cache.Get(cacheKey);

			if (item != null)
				return item;

			item = GetRecentComments(session, 9, ssl);
			cache.Add(cacheKey, item, CachePolicy.AbsoluteExpiration(TimeSpan.FromMinutes(5)));

			return item;

		}

		public SongForApiContract[] GetHighlightedSongs(ContentLanguagePreference languagePreference, SongOptionalFields fields) {
			return HandleQuery(session => GetHighlightedSongs(session).Select(s => new SongForApiContract(s, languagePreference, fields)).ToArray());
		}

		private Song[] GetHighlightedSongs(ISession session) {

			var cacheKey = "OtherService.HighlightedSongs";
			var cachedSongIds = (int[])cache.Get(cacheKey);

			if (cachedSongIds != null) {
				return session.Query<Song>().Where(s => cachedSongIds.Contains(s.Id)).ToArray().OrderByIds(cachedSongIds);
			}

			var cutoffDate = DateTime.Now - TimeSpan.FromDays(2);
			var maxSongs = 1000;
			var songCount = 20;

			// Load at most maxSongs songs for cutoff date
			var recentSongIdAndScore =
				session.Query<Song>()
				.Where(s => !s.Deleted 
					&& s.PVServices != PVServices.Nothing 
					&& s.CreateDate >= cutoffDate
				)
				.OrderByDescending(s => s.CreateDate)
				.Take(maxSongs)
				.Select(s => new {
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
			if (recentSongs.Length >= songCount) {

				songs = recentSongs;

			}  else {

				var moreSongs =
					session.Query<Song>()
					.Where(s => !s.Deleted 
						&& s.PVServices != PVServices.Nothing 						
						&& s.CreateDate < cutoffDate
					)
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
			cache.Add(cacheKey, allSongIds, DateTime.Now + TimeSpan.FromMinutes(15));

			return songs;

		}

		private IEnumerable<EntryWithCommentsContract> CreateEntryWithCommentsContract<T>(IEnumerable<T> comments, Func<T, EntryForApiContract> entryContractFac)
			where T : Comment {
			
			return comments.GroupBy(e => e.Entry, new EntryComparer()).Select(e => new EntryWithCommentsContract(entryContractFac(e.First()), e.Select(c => new CommentContract(c)).ToArray()));

		}

		private IEnumerable<Comment> GetComments<TEntry, TComment>(ISession session, int maxComments, bool checkDeleted) where TComment : GenericComment<TEntry> where TEntry : class, IEntryWithNames {

			var q = session.Query<TComment>();

			if (checkDeleted)
				q = q.Where(c => !c.EntryForComment.Deleted);

			return q.OrderByDescending(c => c.Created).Take(maxComments).ToArray();

		}

		private EntryWithCommentsContract[] GetRecentComments(ISession session, int maxComments, bool ssl) {

			var albumComments = GetComments<Album, AlbumComment>(session, maxComments, true);
			var artistComments = GetComments<Artist, ArtistComment>(session, maxComments, true);
			var songComments = GetComments<Song, SongComment>(session, maxComments, true);
			var discussionComments = GetComments<DiscussionTopic, DiscussionComment>(session, maxComments, true);
			var songListComments = GetComments<SongList, SongListComment>(session, maxComments, false);
			var tagComments = GetComments<Tag, TagComment>(session, maxComments, true);
			var eventComments = GetComments<ReleaseEvent, ReleaseEventComment>(session, maxComments, true);

			// Discussion topics aren't actually comments but we want to show them in the recent comments list anyway
			var discussionTopics = session.Query<DiscussionTopic>().Where(c => !c.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();			
			var discussionTopicsAsComments = discussionTopics.Select(t => new DiscussionComment(t, t.Content, new AgentLoginData(t.Author, t.AuthorName ?? t.Author.Name)) {
				Created = t.Created
			});

			var combined = albumComments
				.Concat(artistComments)
				.Concat(songComments)
				.Concat(songListComments)
				.Concat(discussionComments)
				.Concat(discussionTopicsAsComments)
				.Concat(tagComments)
				.Concat(eventComments)
				.OrderByDescending(c => c.Created)
				.Take(maxComments);
				
			var contracts = CreateEntryWithCommentsContract(combined, c => entryForApiContractFactory.Create(c.Entry, EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture, LanguagePreference, ssl))
				.ToArray();

			return contracts;

		}

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IUserIconFactory userIconFactory, EntryForApiContractFactory entryForApiContractFactory, ObjectCache cache, IEntryThumbPersister thumbPersister) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {
			
			this.userIconFactory = userIconFactory;
			this.entryForApiContractFactory = entryForApiContractFactory;
			this.cache = cache;
			this.thumbPersister = thumbPersister;

		}

		public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {

			HandleTransaction(session => AuditLog(doingWhat, session, who, category));

		}

		public string[] FindNames(SearchTextQuery textQuery, int maxResults) {

			if (textQuery.IsEmpty)
				return new string[] {};

			var artistTextQuery = ArtistSearchTextQuery.Create(textQuery);
			var tagTextQuery = TagSearchTextQuery.Create(textQuery);

			return HandleQuery(session => {

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
					.WhereEntryNameIs(tagTextQuery)
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

		public FrontPageContract GetFrontPageContent(bool ssl) {

			const int maxActivityEntries = 15;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Take(maxActivityEntries)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted);

				var newAlbums = GetRecentAlbums(session, LanguagePreference, AlbumOptionalFields.MainPicture);

				var recentIds = newAlbums.Select(a => a.Id).ToArray();
				var topAlbums = GetTopAlbums(session, recentIds, LanguagePreference, AlbumOptionalFields.MainPicture);

				var newSongs = GetHighlightedSongs(session);

				var firstSongVote = (newSongs.Any() ? session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == newSongs.First().Id && s.User.Id == PermissionContext.LoggedUserId) : null);

				var recentComments = GetRecentComments(session, ssl);

				var recentEvents = GetRecentEvents(session, ssl);

				return new FrontPageContract(activityEntries, newAlbums, recentEvents, recentComments, topAlbums, newSongs, 
					firstSongVote != null ? firstSongVote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference,
					ssl, userIconFactory, PermissionContext, entryForApiContractFactory);

			});

		}

		public IPRule[] GetIPRules() {

			return HandleQuery(session => session.Query<IPRule>().ToArray());

		}

		public EntryWithCommentsContract[] GetRecentComments(bool ssl) {

			return HandleQuery(session => GetRecentComments(session, 50, ssl));

		}

	}
}
