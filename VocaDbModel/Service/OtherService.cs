using System;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Service {

	public class OtherService : ServiceBase {

		private AlbumContract[] GetTopAlbums(ISession session, AlbumContract[] recentAlbums) {

			var cacheKey = "OtherService.PopularAlbums";
			var cache = MemoryCache.Default;
			var item = (TranslatedAlbumContract[])cache.Get(cacheKey);

			if (item != null)
				return item.Select(a => new AlbumContract(a, LanguagePreference)).ToArray();

			var recentIds = recentAlbums.Select(a => a.Id).ToArray();

			var popular = session.Query<Album>()
				.Where(a => !a.Deleted 
					&& a.RatingCount >= 2 && a.RatingAverageInt >= 300	// Filter by number of ratings and average rating
					&& !recentIds.Contains(a.Id))						// Filter out recent albums (that are already shown)
				.OrderByDescending(a => a.RatingTotal)
				.Take(100)
				.ToArray();

			var random = CollectionHelper
				.GetRandomItems(popular, 7)
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

			/*var newAlbums = session.Query<Album>().Where(a => !a.Deleted
				&& a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year < albumCutoffDate.Year
				|| (a.OriginalRelease.ReleaseDate.Year == albumCutoffDate.Year && a.OriginalRelease.ReleaseDate.Month < albumCutoffDate.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == albumCutoffDate.Year
					&& a.OriginalRelease.ReleaseDate.Month == albumCutoffDate.Month
					&& a.OriginalRelease.ReleaseDate.Day < albumCutoffDate.Day)))
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
				.Take(7).ToArray();*/

			return newAlbumContracts;

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

				return recentSongs
					.Concat(moreSongs)
					.OrderByDescending(s => s.RatingScore)
					.ToArray();

			}

		}

		private UnifiedCommentContract[] GetRecentComments(ISession session, int maxComments) {

			var albumComments = session.Query<AlbumComment>().Where(c => !c.Album.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var artistComments = session.Query<ArtistComment>().Where(c => !c.Artist.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();
			var songComments = session.Query<SongComment>().Where(c => !c.Song.Deleted).OrderByDescending(c => c.Created).Take(maxComments).ToArray();

			var combined = albumComments.Cast<Comment>().Concat(artistComments).Concat(songComments)
				.OrderByDescending(c => c.Created)
				.Take(maxComments)
				.Select(c => new UnifiedCommentContract(c, LanguagePreference));

			return combined.ToArray();

		}

		public OtherService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

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
					.FilterByArtistName(artistTextQuery)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumNames = session.Query<AlbumName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songNames = session.Query<SongName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var tagNames = session.Query<Tag>()
					.AddTagNameFilter(tagTextQuery)
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
					.FilterByArtistName(artistTextQuery)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var artistCount = (getTotalCount ?
					session.Query<ArtistName>()
					.FilterByArtistName(artistTextQuery)
					.Where(a => !a.Artist.Deleted)
					.Select(n => n.Artist)
					.Distinct()
					.Count() 
					: 0);

				var albums = 
					session.Query<AlbumName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var albumCount = (getTotalCount ?
					session.Query<AlbumName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Album.Deleted)
					.Select(n => n.Album)
					.Distinct()
					.Count()
					: 0);

				var songs = 
					session.Query<SongName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.AddNameOrder(LanguagePreference)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				var songCount = (getTotalCount ?
					session.Query<SongName>()
					.AddEntryNameFilter(textQuery)
					.Where(a => !a.Song.Deleted)
					.Select(n => n.Song)
					.Distinct()
					.Count()
					: 0);

				var tags = session.Query<Tag>()
					.AddTagNameFilter(tagTextQuery)
					.OrderBy(t => t.Name)
					.Take(maxResults)
					.ToArray();

				var tagCount = (getTotalCount ? session.Query<Tag>()
					.AddTagNameFilter(tagTextQuery)
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

		public FrontPageContract GetFrontPageContent() {

			const int maxNewsEntries = 4;
			const int maxActivityEntries = 15;

			return HandleQuery(session => {

				var activityEntries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Take(maxActivityEntries)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted);

				var newsEntries = session.Query<NewsEntry>().Where(n => n.Stickied).OrderByDescending(a => a.CreateDate).Take(maxNewsEntries).ToArray();

				if (newsEntries.Length < maxNewsEntries)
					newsEntries = newsEntries.Concat(session.Query<NewsEntry>()
						.Where(n => !n.Stickied)
						.OrderByDescending(a => a.CreateDate)
						.Take(maxNewsEntries - newsEntries.Length)).ToArray();

				var newAlbums = GetRecentAlbums(session);

				var topAlbums = GetTopAlbums(session, newAlbums);

				var newSongs = GetHighlightedSongs(session);

				var firstSongVote = (newSongs.Any() ? session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == newSongs.First().Id && s.User.Id == PermissionContext.LoggedUserId) : null);

				var recentComments = GetRecentComments(session, 7);

				return new FrontPageContract(activityEntries, newsEntries, newAlbums, recentComments, topAlbums, newSongs, 
					firstSongVote != null ? firstSongVote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference);

			});

		}

		public IPRule[] GetIPRules() {

			return HandleQuery(session => session.Query<IPRule>().ToArray());

		}

		public UnifiedCommentContract[] GetRecentComments() {

			return HandleQuery(session => GetRecentComments(session, 50));

		}

	}
}
