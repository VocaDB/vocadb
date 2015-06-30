using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
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

		private readonly IEntryThumbPersister thumbPersister;

		private AlbumContract[] GetTopAlbums(ISession session, AlbumContract[] recentAlbums) {

			var minRatings = 2;
			var sampleSize = 300;
			var cacheKey = "OtherService.PopularAlbums";
			var cache = MemoryCache.Default;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumContract(a, LanguagePreference)).ToArray();

			var recentIds = recentAlbums.Select(a => a.Id).ToArray();

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
				.GetRandomItems(popularIds, 7)
				.ToArray();

			var random = session.Query<Album>()
				.Where(a => randomIds.Contains(a.Id))
				.OrderByDescending(a => a.RatingAverageInt)
				.ToArray();

			var popularAlbumsCached = random
				.Select(a => new TranslatedAlbumContract(a))
				.ToArray();

			var popularAlbumContracts = random
				.Select(a => new AlbumContract(a, LanguagePreference))
				.ToArray();

			cache.Add(cacheKey, popularAlbumsCached, DateTime.Now + TimeSpan.FromHours(24));

			return popularAlbumContracts;

		}

		private AlbumContract[] GetRecentAlbums(ISession session) {

			var cacheKey = "OtherService.RecentAlbums";
			var cache = MemoryCache.Default;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumContract(a, LanguagePreference)).ToArray();

			var now = DateTime.Now;

			var upcoming = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year > now.Year
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year && a.OriginalRelease.ReleaseDate.Month > now.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year
					&& a.OriginalRelease.ReleaseDate.Month == now.Month
					&& a.OriginalRelease.ReleaseDate.Day > now.Day)))
				.OrderBy(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenBy(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(4).ToArray();

			var recent = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year < now.Year
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year && a.OriginalRelease.ReleaseDate.Month < now.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == now.Year
					&& a.OriginalRelease.ReleaseDate.Month == now.Month
					&& a.OriginalRelease.ReleaseDate.Day <= now.Day)))
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(3).ToArray();

			var newAlbums = upcoming.Reverse().Concat(recent)
				.Select(a => new TranslatedAlbumContract(a))
				.ToArray();

			var newAlbumContracts = upcoming.Reverse().Concat(recent)
				.Select(a => new AlbumContract(a, LanguagePreference))
				.ToArray();

			cache.Add(cacheKey, newAlbums, DateTime.Now + TimeSpan.FromHours(1));

			return newAlbumContracts;

		}

		private EntryWithCommentsContract[] GetRecentComments(ISession session, bool ssl) {
			
			var cacheKey = string.Format("OtherService.RecentComments.{0}.{1}", LanguagePreference, ssl);
			var cache = MemoryCache.Default;
			var item = (EntryWithCommentsContract[])cache.Get(cacheKey);

			if (item != null)
				return item;

			item = GetRecentComments(session, 9, ssl);
			cache.Add(cacheKey, item, DateTime.Now + TimeSpan.FromMinutes(10));

			return item;

		}

		private Song[] GetHighlightedSongs(ISession session) {

			var cutoffDate = DateTime.Now - TimeSpan.FromDays(2);
			var maxSongs = 100;
			var songCount = 20;

			var recentSongs =
				session.Query<Song>()
				.Where(s => !s.Deleted 
					&& s.PVServices != PVServices.Nothing 
					&& s.CreateDate >= cutoffDate
				)
				.OrderByDescending(s => s.CreateDate)
				.Take(maxSongs)
				.ToArray();

			if (recentSongs.Length >= songCount) {
				
				return recentSongs
					.OrderByDescending(s => s.RatingScore)
					.Take(songCount)
					.ToArray();

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

				return 
					recentSongs
					.Concat(moreSongs)
					.OrderByDescending(s => s.RatingScore)
					.ToArray();

			}

		}

		private IEnumerable<EntryWithCommentsContract> CreateEntryWithCommentsContract<T>(IEnumerable<T> comments, Func<T, EntryForApiContract> entryContractFac)
			where T : Comment {
			
			return comments.GroupBy(e => e.Entry, new EntryComparer()).Select(e => new EntryWithCommentsContract(entryContractFac(e.First()), e.Select(c => new CommentContract(c)).ToArray()));

		}

		private EntryWithCommentsContract[] GetRecentComments(ISession session, int maxComments, bool ssl) {

			var albumComments = session.Query<AlbumComment>().Where(c => !c.Album.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var artistComments = session.Query<ArtistComment>().Where(c => !c.Artist.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var songComments = session.Query<SongComment>().Where(c => !c.Song.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();			
			var discussionComments = session.Query<DiscussionComment>().Where(c => !c.Topic.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();			

			// Discussion topics aren't actually comments but we want to show them in the recent comments list anyway
			var discussionTopics = session.Query<DiscussionTopic>().Where(c => !c.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();			
			var discussionTopicsAsComments = discussionTopics.Select(t => new DiscussionComment(t, t.Content, new AgentLoginData(t.Author, t.AuthorName ?? t.Author.Name)) {
				Created = t.Created
			});

			var combined = albumComments.Cast<Comment>().Concat(artistComments).Concat(songComments).Concat(discussionComments).Concat(discussionTopicsAsComments)
				.OrderByDescending(c => c.Created)
				.Take(maxComments);
				
			var contracts = CreateEntryWithCommentsContract(combined, c => EntryForApiContract.Create(c.Entry, LanguagePreference, thumbPersister, null, ssl, EntryOptionalFields.MainPicture))
				.ToArray();

			return contracts;

		}

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IEntryThumbPersister thumbPersister) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {
			
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

				var tagNames = session.Query<Tag>()
					.WhereHasName(tagTextQuery)
					.OrderBy(t => t.Name)
					.Select(t => t.Name)
					.Take(maxResults)
					.ToArray();

				var allNames = artistNames
					.Concat(albumNames)
					.Concat(songNames)
					.Concat(tagNames)
					.Distinct()
					.OrderBy(n => n)
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(allNames, textQuery.Query);

			});

		}

		public AllEntriesSearchResult Find(string query, int maxResults, bool getTotalCount) {

			if (string.IsNullOrWhiteSpace(query))
				return new AllEntriesSearchResult();

			var textQuery = SearchTextQuery.Create(query);
			var artistTextQuery = ArtistSearchTextQuery.Create(query, textQuery.MatchMode); // Can't use the existing words collection here as they are noncanonized
			var tagTextQuery = TagSearchTextQuery.Create(query, textQuery.MatchMode);

			return HandleQuery(session => {

				var artists = 
					session.Query<ArtistName>()
					.WhereArtistNameIs(artistTextQuery)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.OrderByEntryName(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var artistCount = (getTotalCount ?
					session.Query<ArtistName>()
					.WhereArtistNameIs(artistTextQuery)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.Distinct()
					.Count() 
					: 0);

				var albums = 
					session.Query<AlbumName>()
					.WhereEntryNameIs(textQuery)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.OrderByEntryName(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumCount = (getTotalCount ?
					session.Query<AlbumName>()
					.WhereEntryNameIs(textQuery)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.Distinct()
					.Count()
					: 0);

				var songs = 
					session.Query<SongName>()
					.WhereEntryNameIs(textQuery)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.OrderByEntryName(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songCount = (getTotalCount ?
					session.Query<SongName>()
					.WhereEntryNameIs(textQuery)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.Distinct()
					.Count()
					: 0);

				var tags = session.Query<Tag>()
					.WhereHasName(tagTextQuery)
					.OrderBy(t => t.Name)
					.Take(maxResults)
					.ToArray();

				var tagCount = (getTotalCount ? session.Query<Tag>()
					.WhereHasName(tagTextQuery)
					.Distinct()
					.Count()
					: 0);

				var artistResult = new PartialFindResult<ArtistContract>(
					artists.Select(a => new ArtistContract(a, PermissionContext.LanguagePreference)).ToArray(), artistCount);

				var albumResult = new PartialFindResult<AlbumContract>(
					albums.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference)).ToArray(), albumCount);

				var songResult = new PartialFindResult<SongWithAlbumContract>(
					songs.Select(a => new SongWithAlbumContract(a, PermissionContext.LanguagePreference)).ToArray(), songCount);

				var tagResult = new PartialFindResult<TagContract>(
					tags.Select(a => new TagContract(a)).ToArray(), tagCount);

				return new AllEntriesSearchResult(query, albumResult, artistResult, songResult, tagResult);

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

				var newAlbums = GetRecentAlbums(session);

				var topAlbums = GetTopAlbums(session, newAlbums);

				var newSongs = GetHighlightedSongs(session);

				var firstSongVote = (newSongs.Any() ? session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == newSongs.First().Id && s.User.Id == PermissionContext.LoggedUserId) : null);

				var recentComments = GetRecentComments(session, ssl);

				return new FrontPageContract(activityEntries, newAlbums, recentComments, topAlbums, newSongs, 
					firstSongVote != null ? firstSongVote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference);

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
