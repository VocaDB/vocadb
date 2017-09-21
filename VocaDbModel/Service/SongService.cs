using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

#pragma warning disable 169
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
#pragma warning restore 169

		private readonly VdbConfigManager config;
		private readonly IEntryUrlParser entryUrlParser;

		private PartialFindResult<Song> Find(ISession session, SongQueryParams queryParams) {
			return new SongSearch(new NHibernateDatabaseContext(session, PermissionContext), queryParams.LanguagePreference, entryUrlParser).Find(queryParams);
		}

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IEntryUrlParser entryUrlParser,
			VdbConfigManager config)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

			this.entryUrlParser = entryUrlParser;
			this.config = config;

		}

		public void AddSongToList(int listId, int songId, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var list = session.Load<SongList>(listId);
				var items = session.Query<SongInList>().Where(s => s.List.Id == listId);
				int order = 1;

				if (items.Any())
					order = items.Max(s => s.Order) + 1;

				EntryPermissionManager.VerifyEdit(PermissionContext, list);

				var song = session.Load<Song>(songId);

				var link = list.AddSong(song, order, notes);
				session.Save(link);

				SysLog(string.Format("added {0} to {1}", song, list));

			});

		}

		public void Archive(ISession session, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Song song, SongArchiveReason reason, string notes = "") {

			Archive(session, song, new SongDiff(), reason, notes);

		}

		public void Delete(int id, string notes) {

			UpdateEntity<Song>(id, (session, song) => {

				EntryPermissionManager.VerifyDelete(PermissionContext, song);

				AuditLog(string.Format("deleting song {0}", EntryLinkFactory.CreateEntryLink(song)), session);

				song.Delete();

				Archive(session, song, new SongDiff(false), SongArchiveReason.Deleted, notes);

			}, PermissionToken.Nothing, skipLog: true);

		}

		public T FindFirst<T>(Func<Song, ISession, T> fac, string[] query, NameMatchMode nameMatchMode)
			where T : class {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session,
						new SongQueryParams {
							Common = new CommonSearchParams {
								TextQuery = SearchTextQuery.Create(q, nameMatchMode),
								OnlyByName = true, MoveExactToTop = true
							},
							LanguagePreference = LanguagePreference,
							Paging = new PagingProperties(0, 30, false)
						});

					if (result.Items.Any())
						return fac(result.Items.First(), session);

				}

				return null;

			});

		}

		public SongDetailsContract FindFirstDetails(SearchTextQuery textQuery) {

			return FindFirst((s, session) => new SongDetailsContract(s, PermissionContext.LanguagePreference, new SongListBaseContract[0], config.SpecialTags, PermissionContext, null), 
				new[]{ textQuery.Query }, textQuery.MatchMode);

		}

		public PartialFindResult<T> Find<T>(Func<Song, T> fac, SongQueryParams queryParams)
			where T : class {

			return HandleQuery(session => {

				var result = Find(session, queryParams);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
					result.TotalCount, result.Term);

			});

		}

		public PartialFindResult<SongContract> Find(SongQueryParams queryParams) {

			return Find(s => new SongContract(s, PermissionContext.LanguagePreference), queryParams);

		}

		public PartialFindResult<SongWithAlbumAndPVsContract> FindWithAlbum(SongQueryParams queryParams, bool getPVs) {

			return Find(s => new SongWithAlbumAndPVsContract(s, PermissionContext.LanguagePreference, getPVs), queryParams);

		}

		public PartialFindResult<SongContract> FindWithThumbPreferNotNico(SongQueryParams queryParams) {

			return Find(s => new SongContract(s, PermissionContext.LanguagePreference, VideoServiceHelper.GetThumbUrlPreferNotNico(s.PVs.PVs)), queryParams);

		}

		[Obsolete]
		public SongContract[] FindByName(string term, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Song>()
					.Where(s =>
						!s.Deleted &&
						(string.IsNullOrEmpty(term)
							|| s.Names.SortNames.English.Contains(term)
							|| s.Names.SortNames.Romaji.Contains(term)
							|| s.Names.SortNames.Japanese.Contains(term)
						|| (s.NicoId != null && s.NicoId == term)))
					.Take(maxResults)
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(term) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				return direct.Concat(additionalNames)
					.Take(maxResults)
					.Select(a => new SongContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public string[] FindNames(SearchTextQuery textQuery, int maxResults) {

			if (textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session => {

				var names = session.Query<SongName>()
					.Where(a => !a.Song.Deleted)
					.WhereEntryNameIs(textQuery)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(names, textQuery.Query);

			});

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				return new EntryWithTagUsagesContract(song, song.Tags.ActiveUsages, LanguagePreference, PermissionContext);

			});

		}

		public LyricsForSongContract GetRandomLyricsForSong(string query) {

			return HandleQuery(session => {

				var songContract = Find(session, new SongQueryParams(SearchTextQuery.Create(query), 
					new SongType[] {}, 0, 10, false, 
					SongSortRule.Name, false, true, null) {
					AdvancedFilters = new[] { new AdvancedSearchFilter { FilterType = AdvancedFilterType.Lyrics, Param = AdvancedSearchFilter.Any } }
				}).Items;

				if (!songContract.Any())
					return null;
				
				var songIds = songContract.Select(s => s.Id).ToArray();

				var songs = session.Query<Song>().Where(s => songIds.Contains(s.Id)).ToArray();
				var allLyrics = songs.SelectMany(s => s.Lyrics).ToArray();

				if (!allLyrics.Any())
					return null;

				var lyrics = allLyrics[new Random().Next(allLyrics.Length)];

				return new LyricsForSongContract(lyrics);

			});

		}

		public LyricsForSongContract GetRandomSongWithLyricsDetails() {

			return HandleQuery(session => {

				var ids = session.Query<LyricsForSong>().Select(s => s.Id).ToArray();
				var id = ids[new Random().Next(ids.Length)];

				return new LyricsForSongContract(session.Load<LyricsForSong>(id));

			});

		}

		public T GetSong<T>(int id, Func<Song, T> fac) {

			return HandleQuery(session => fac(session.Load<Song>(id)));

		}

		public SongListBaseContract[] GetSongListsForCurrentUser(int ignoreSongId) {

			PermissionContext.VerifyLogin();

			var canEditPools = PermissionContext.HasPermission(PermissionToken.EditFeaturedLists);

			return HandleQuery(session => {

				var ignoredLists = session
					.Query<SongInList>()
					.Where(sil => sil.Song.Id == ignoreSongId)
					.Select(sil => sil.List.Id)
					.Distinct()
					.ToArray();

				return session.Query<SongList>()
					.WhereNotDeleted()
					.Where(l => !ignoredLists.Contains(l.Id) && 
						((l.Author.Id == PermissionContext.LoggedUser.Id && l.FeaturedCategory == SongListFeaturedCategory.Nothing) 
							|| (canEditPools && l.FeaturedCategory == SongListFeaturedCategory.Pools)))
					.OrderBy(l => l.Name)
					.ToArray()
					.Select(l => new SongListBaseContract(l))
					.ToArray();

			});

		}

		public SongListContract[] GetPublicSongListsForSong(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				var userId = PermissionContext.LoggedUserId;
				return song.ListLinks
					.Where(l => l.List.FeaturedCategory != SongListFeaturedCategory.Nothing || l.List.Author.Id == userId || l.List.Author.Options.PublicRatings)
					.OrderBy(l => l.List.Name)
					.Select(l => l.List)
					.Distinct()
					.Select(l => new SongListContract(l, PermissionContext))
					.ToArray();

			});

		}

		public SongContract GetSongWithAdditionalNames(int id) {

			return HandleQuery(
				session => new SongContract(session.Load<Song>(id), PermissionContext.LanguagePreference));

		}

		public SongWithArchivedVersionsContract GetSongWithArchivedVersions(int songId) {

			return HandleQuery(session => new SongWithArchivedVersionsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public T GetSongWithPV<T>(Func<Song, T> fac, PVService service, string pvId) 
			where T : class {

			return HandleQuery(session => {

				var pv = session.Query<PVForSong>()
					.FirstOrDefault(p => p.Service == service && p.PVId == pvId && !p.Song.Deleted);

				return (pv != null ? fac(pv.Song) : null);

			});

		}

		public SongContract GetSongWithPV(PVService service, string pvId) {

			return GetSongWithPV(s => new SongContract(s, PermissionContext.LanguagePreference), service, pvId);

		}

		public SongWithAlbumContract GetSongWithPVAndAlbum(PVService service, string pvId) {

			return GetSongWithPV(s => new SongWithAlbumContract(s, PermissionContext.LanguagePreference), service, pvId);

		}

		public SongContract[] GetSongs(string filter, int start, int count) {

			return HandleQuery(session => session.Query<Song>()
				.Where(s => string.IsNullOrEmpty(filter)
					|| s.Names.SortNames.Japanese.Contains(filter) 
					|| s.NicoId == filter)
				.OrderBy(s => s.Names.SortNames.Japanese)
				.Skip(start)
				.Take(count)
				.ToArray()
				.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public ArchivedSongVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedSongVersionDetailsContract(session.Load<ArchivedSongVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedSongVersion>(comparedVersionId) : null, 
					PermissionContext.LanguagePreference));

		}

		public XDocument GetVersionXml(int id) {
			return HandleQuery(session => session.Load<ArchivedSongVersion>(id).Data);
		}

		public void Restore(int songId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				song.Deleted = false;

				Archive(session, song, new SongDiff(false), SongArchiveReason.Restored);

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(song), session);

			});

		}

		public SongDetailsContract XGetSongByNameArtistAndAlbum(string name, string artist, string album) {

			return HandleQuery(session => {

				var matches = session.Query<SongName>().Where(n => n.Value == name)
					.Select(n => n.Song)
					.ToArray();

				Artist[] artists = null;

				if (!string.IsNullOrEmpty(artist)) {

					artists = session.Query<ArtistName>()
						.WhereArtistNameIs(ArtistSearchTextQuery.Create(artist))
						.Select(n => n.Artist)
						.Take(10)
						.ToArray();

				}

				if (artists != null && artists.Any())
					matches = matches.Where(s => s.ArtistList.Any(a => artists.Contains(a))).ToArray();

				Album[] albums = null;

				if (!string.IsNullOrEmpty(album)) {

					albums = session.Query<AlbumName>()
						.WhereEntryNameIs(SearchTextQuery.Create(album))
						.Select(n => n.Album)
						.Take(10)
						.ToArray();

				}

				if (albums != null && albums.Any())
					matches = matches.Where(s => s.Albums.Any(a => albums.Contains(a.Album))).ToArray();

				if (matches.Length == 1)
					return new SongDetailsContract(matches.First(), PermissionContext.LanguagePreference, new SongListBaseContract[0], null, PermissionContext, null);

				if (matches.Length == 0)
					return null;

				matches = session.Query<SongName>()
					.WhereEntryNameIs(SearchTextQuery.Create(name))
					.Select(n => n.Song)
					.ToArray();

				if (artists != null && artists.Any())
					matches = matches.Where(s => s.ArtistList.Any(a => artists.Contains(a))).ToArray();

				if (albums != null && albums.Any())
					matches = matches.Where(s => s.Albums.Any(a => albums.Contains(a.Album))).ToArray();

				if (matches.Length == 1)
					return new SongDetailsContract(matches.First(), PermissionContext.LanguagePreference, new SongListBaseContract[0], null, PermissionContext, null);

				return null;

			});

		}

	}

	public enum SongSortRule {

		None,

		Name,

		AdditionDate,

		PublishDate,

		FavoritedTimes,

		RatingScore

	}

}
