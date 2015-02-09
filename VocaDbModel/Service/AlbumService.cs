using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using NLog;
using VocaDb.Model.DataContracts.PVs;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Artists;
using System.Drawing;
using System;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.TagFormatting;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
// ReSharper restore UnusedMember.Local

		private PartialFindResult<Album> Find(ISession session, AlbumQueryParams queryParams) {

			return new AlbumSearch(new QuerySourceSession(session), LanguagePreference).Find(queryParams);

		}

		private PartialFindResult<Album> FindAdvanced(
			ISession session, string query, PagingProperties paging, AlbumSortRule sortRule) {

			var queryPlan = new AlbumQueryBuilder().BuildPlan(query);
			return FindAdvanced(session, queryPlan, paging, sortRule);

		}

		private PartialFindResult<Album> FindAdvanced(
			ISession session, QueryPlan<Album> queryPlan, PagingProperties paging, AlbumSortRule sortRule) {

			var querySource = new QuerySourceSession(session);
			var processor = new QueryProcessor<Album>(querySource);

			return processor.Query(queryPlan, paging, q => AlbumSearchSort.AddOrder(q, sortRule, LanguagePreference));

		}

		private AlbumMergeRecord GetMergeRecord(ISession session, int sourceId) {
			return session.Query<AlbumMergeRecord>().FirstOrDefault(s => s.Source == sourceId);
		}

		private ArtistForAlbum RestoreArtistRef(Album album, Artist artist, ArchivedArtistForAlbumContract albumRef) {

			if (artist != null) {

				return (!artist.HasAlbum(album) ? artist.AddAlbum(album, albumRef.IsSupport, albumRef.Roles) : null);

			} else {

				return album.AddArtist(albumRef.NameHint, albumRef.IsSupport, albumRef.Roles);

			}

		}

		private SongInAlbum RestoreTrackRef(Album album, Song song, SongInAlbumRefContract songRef) {

			if (song != null) {

				return (!album.HasSong(song) ? album.AddSong(song, songRef.TrackNumber, songRef.DiscNumber) : null);

			} else {

				return album.AddSong(songRef.NameHint, songRef.TrackNumber, songRef.DiscNumber);

			}

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext,entryLinkFactory) {}

		public void Archive(ISession session, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Album album, AlbumArchiveReason reason, string notes = "") {

			Archive(session, album, new AlbumDiff(), reason, notes);

		}

		public bool CreateReport(int albumId, AlbumReportType reportType, string hostname, string notes) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(session => {

				var loggedUserId = PermissionContext.LoggedUserId;
				var existing = session.Query<AlbumReport>()
					.FirstOrDefault(r => r.Album.Id == albumId 
						&& ((loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname));

				if (existing != null)
					return false;

				var album = session.Load<Album>(albumId);
				var report = new AlbumReport(album, reportType, GetLoggedUserOrDefault(session), hostname, notes.Truncate(EntryReport.MaxNotesLength));

				var msg = string.Format("reported {0} as {1} ({2})", EntryLinkFactory.CreateEntryLink(album), reportType, HttpUtility.HtmlEncode(notes));
				AuditLog(msg.Truncate(200), session, new AgentLoginData(GetLoggedUserOrDefault(session), hostname));

				session.Save(report);
				return true;

			}, IsolationLevel.ReadUncommitted);

		}

		public void Delete(int id) {

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting album {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				NHibernateUtil.Initialize(a.CoverPictureData);
				a.Delete();

			}, PermissionToken.DeleteEntries, skipLog: true);

		}

		/*
		// TODO: should be moved to update
		public void DeleteArtistForAlbum(int artistForAlbumId) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);

				AuditLog(string.Format("deleting {0}", artistForAlbum), session);

				artistForAlbum.Album.DeleteArtistForAlbum(artistForAlbum);
				session.Delete(artistForAlbum);
				session.Update(artistForAlbum.Album);

				SysLog(string.Format("deleted {0} successfully", artistForAlbum));

			});

		}*/

		public PartialFindResult<T> Find<T>(Func<Album, T> fac, AlbumQueryParams queryParams)
			where T : class {

			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session => {

				var result = Find(session, queryParams);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
					result.TotalCount, result.Term, result.FoundExactMatch);

			});

		}

		public PartialFindResult<AlbumContract> Find(AlbumQueryParams queryParams) {

			return Find(s => new AlbumContract(s, LanguagePreference), queryParams);

		}

		public PartialFindResult<AlbumContract> Find(
			SearchTextQuery textQuery, DiscType discType, int start, int maxResults, bool draftsOnly, bool getTotalCount, 
			AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false) {

			var queryParams = new AlbumQueryParams(textQuery, discType, start, maxResults, draftsOnly, getTotalCount, sortRule, moveExactToTop);
			return Find(queryParams);

		}

		public PartialFindResult<AlbumContract> FindAdvanced(
			string query, PagingProperties paging, AlbumSortRule sortRule) {

			return HandleQuery(session => {

				var results = FindAdvanced(session, query, paging, sortRule);

				return new PartialFindResult<AlbumContract>(
					results.Items.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference)).ToArray(),
					results.TotalCount, results.Term, results.FoundExactMatch);

			});

		}

		public int? FindByMikuDbId(int mikuDbId) {

			return HandleQuery(session => {

				var link = session.Query<AlbumWebLink>()
					.FirstOrDefault(w => !w.Album.Deleted && w.Url.Contains("mikudb.com/" + mikuDbId + "/"));

				return (link != null ? (int?)link.Album.Id : null);

			});

		}

		/*
		public AlbumWithAdditionalNamesContract FindByNames(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session, q, DiscType.Unknown, 0, 1, false, false, NameMatchMode.Exact, AlbumSortRule.Name, false);

					if (result.Items.Any())
						return new AlbumWithAdditionalNamesContract(result.Items.First(), PermissionContext.LanguagePreference);

				}

				return null;

			});

		}*/

		public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName) {

			var names = anyName.Select(n => n.Trim()).Where(n => n != string.Empty).ToArray();

			if (!names.Any())
				return new EntryRefWithCommonPropertiesContract[] { };

			// TODO: moved Distinct after ToArray to work around NH bug
			return HandleQuery(session => {

				return session.Query<AlbumName>()
					.Where(n => names.Contains(n.Value))
					.Select(n => n.Album)
					.Where(n => !n.Deleted)
					.Take(10)
					.ToArray()
					.Distinct()
					.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public string[] FindNames(SearchTextQuery textQuery, int maxResults) {

			if (textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session => {

				var names = session.Query<AlbumName>()
					.Where(a => !a.Album.Deleted)
					.AddEntryNameFilter(textQuery)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(names, textQuery.Query);

			});

		}

		public T GetAlbum<T>(int id, Func<Album, T> fac) {

			return HandleQuery(session => fac(session.Load<Album>(id)));

		}

		public T GetAlbumWithMergeRecord<T>(int id, Func<Album, AlbumMergeRecord, T> fac) {

			return HandleQuery(session => {
				var album = session.Load<Album>(id);
				return fac(album, (album.Deleted ? GetMergeRecord(session, id) : null));
			});

		}

		public AlbumContract GetAlbum(int id) {

			return GetAlbum(id, a => new AlbumContract(a, PermissionContext.LanguagePreference));

		}

		public AlbumContract GetAlbumByLink(string link) {

			return HandleQuery(session => {

				var webLink = session.Query<AlbumWebLink>().FirstOrDefault(p => p.Url.Contains(link));

				return (webLink != null ? new AlbumContract(webLink.Album, PermissionContext.LanguagePreference) : null);

			});

		}

		/// <summary>
		/// Gets album details, and updates hit count if necessary.
		/// </summary>
		/// <param name="id">Id of the album to be retrieved.</param>
		/// <param name="hostname">
		/// Hostname of the user requestin the album. Used to hit counting when no user is logged in. If null or empty, and no user is logged in, hit count won't be updated.
		/// </param>
		/// <returns>Album details contract. Cannot be null.</returns>
		public AlbumDetailsContract GetAlbumDetails(int id, string hostname) {

			return HandleQuery(session => {

				var album = session.Load<Album>(id);

				var stats = session.Query<Album>()
					.Where(a => a.Id == id)
					.Select(a => new {
						OwnedCount = a.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Owned),
						WishlistedCount = a.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Wishlisted),
						CommentCount = a.Comments.Count,
						Hits = a.Hits.Count
					})
					.FirstOrDefault();

				if (stats == null)
					throw new ObjectNotFoundException(id, typeof(Album));

				var contract = new AlbumDetailsContract(album, PermissionContext.LanguagePreference) {
					OwnedCount = stats.OwnedCount,
					WishlistCount = stats.WishlistedCount,
					CommentCount = stats.CommentCount,
					Hits = stats.Hits
				};

				var user = PermissionContext.LoggedUser;

				if (user != null) {

					var albumForUser = session.Query<AlbumForUser>()
						.FirstOrDefault(a => a.Album.Id == id && a.User.Id == user.Id);

					contract.AlbumForUser = (albumForUser != null ? new AlbumForUserContract(albumForUser, PermissionContext.LanguagePreference) : null);

				}

				contract.LatestComments = session.Query<AlbumComment>()
					.Where(c => c.Album.Id == id)
					.OrderByDescending(c => c.Created)
					.Take(3)
					.ToArray()
					.Select(c => new CommentContract(c))
					.ToArray();

				if (album.Deleted) {
					var mergeEntry = GetMergeRecord(session, id);
					contract.MergedTo = (mergeEntry != null ? new AlbumContract(mergeEntry.Target, LanguagePreference) : null);
				}

				if (user != null || !string.IsNullOrEmpty(hostname)) {

					var agentNum = (user != null ? user.Id : hostname.GetHashCode());

					using (var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted)) {

						var isHit = session.Query<AlbumHit>().Any(h => h.Album.Id == id && h.Agent == agentNum);

						if (!isHit) {

							var hit = new AlbumHit(album, agentNum);
							session.Save(hit);

							try {
								tx.Commit();
							} catch (SqlException x) {
								log.Warn("Error while committing hit", x);
							}

						}

					}

				}


				return contract;

			});

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public string GetAlbumTagString(int id, string format, bool includeHeader) {

			return GetAlbum(id, a => new TagFormatter().ApplyFormat(a, format, PermissionContext.LanguagePreference, includeHeader));

		}

		public AlbumContract GetAlbumWithAdditionalNames(int id) {

			return HandleQuery(session => new AlbumContract(
				session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumWithArchivedVersionsContract GetAlbumWithArchivedVersions(int albumId) {

			return HandleQuery(session => 
				new AlbumWithArchivedVersionsContract(session.Load<Album>(albumId), PermissionContext.LanguagePreference));

		}

		public EntryForPictureDisplayContract GetArchivedAlbumPicture(int archivedVersionId) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(
				session.Load<ArchivedAlbumVersion>(archivedVersionId), PermissionContext.LanguagePreference));

		}

		/// <summary>
		/// Gets the cover picture for a <see cref="Album"/>.
		/// </summary>
		/// <param name="id">Album Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public EntryForPictureDisplayContract GetCoverPicture(int id, Size requestedSize) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(session.Load<Album>(id), PermissionContext.LanguagePreference, requestedSize));

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int albumId) {

			return HandleQuery(session => { 
				
				var album = session.Load<Album>(albumId);
				return new EntryWithTagUsagesContract(album, album.Tags.Usages);

			});

		}

		public TagSelectionContract[] GetTagSelections(int albumId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<AlbumTagUsage>().Where(a => a.Album.Id == albumId).ToArray();
				var tagVotes = session.Query<AlbumTagVote>().Where(a => a.User.Id == userId && a.Usage.Album.Id == albumId).ToArray();

				var tagSelections = tagsInUse.Select(t => 
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		[Obsolete("Replaced by saving properties")]
		public TrackPropertiesContract GetTrackProperties(int albumId, int songId) {

			return HandleQuery(session => {

				var artists = session.Query<ArtistForAlbum>()
					.Where(a => a.Album.Id == albumId && a.Artist != null && !a.Artist.Deleted 
						&& ArtistHelper.SongArtistTypes.Contains(a.Artist.ArtistType))
					.Select(a => a.Artist)
					.ToArray();
				var song = session.Load<Song>(songId);

				return new TrackPropertiesContract(song, 
					artists, PermissionContext.LanguagePreference);

			});

		}

		public AlbumForUserContract[] GetUsersWithAlbumInCollection(int albumId) {

			return HandleQuery(session => 
				
				session.Load<Album>(albumId)
					.UserCollections
			        .Where(a => a.PurchaseStatus != PurchaseStatus.Nothing && a.User.Options.PublicAlbumCollection)
					.OrderBy(u => u.User.Name)
					.Select(u => new AlbumForUserContract(u, LanguagePreference)).ToArray());

		}

		public ArchivedAlbumVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedAlbumVersionDetailsContract(session.Load<ArchivedAlbumVersion>(id), 
					(comparedVersionId != 0 ? session.Load<ArchivedAlbumVersion>(comparedVersionId) : null), PermissionContext.LanguagePreference));

		}

		public XDocument GetVersionXml(int id) {
			return HandleQuery(session => session.Load<ArchivedAlbumVersion>(id).Data);
		}

		public int MoveToTrash(int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("moving {0} to trash", album), session);

				NHibernateUtil.Initialize(album.CoverPictureData);

				var archived = new ArchivedAlbumContract(album, new AlbumDiff(true));
				var data = XmlHelper.SerializeToXml(archived);
				var trashed = new TrashedEntry(album, data, GetLoggedUser(session));

				session.Save(trashed);

				album.DeleteLinks();
				session.Delete(album);

				return trashed.Id;

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return RemoveTagUsage<AlbumTagUsage>(tagUsageId);

		}

		public void Restore(int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				album.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(album), session);

			});

		}

		public EntryRevertedContract RevertToVersion(int archivedAlbumVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedAlbumVersion>(archivedAlbumVersionId);
				var album = archivedVersion.Album;

				SysLog("reverting " + album + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedAlbumContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				album.Description.Original = fullProperties.Description;
				album.Description.English = fullProperties.DescriptionEng;
				album.DiscType = fullProperties.DiscType;
				album.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);

				if (versionWithPic != null)
					album.CoverPictureData = versionWithPic.CoverPicture;

				// Original release
				album.OriginalRelease = (fullProperties.OriginalRelease != null ? new AlbumRelease(fullProperties.OriginalRelease) : null);

				// Artists
				SessionHelper.RestoreObjectRefs<ArtistForAlbum, Artist, ArchivedArtistForAlbumContract>(
					session, warnings, album.AllArtists, fullProperties.Artists, 
					(a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(artist, albumRef) => RestoreArtistRef(album, artist, albumRef),
					albumForArtist => albumForArtist.Delete());

				// Songs
				SessionHelper.RestoreObjectRefs<SongInAlbum, Song, SongInAlbumRefContract>(
					session, warnings, album.AllSongs, fullProperties.Songs, 
					(a1, a2) => ((a1.Song != null && a1.Song.Id == a2.Id) || a1.Song == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(song, songRef) => RestoreTrackRef(album, song, songRef),
					songInAlbum => songInAlbum.Delete());

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = album.Names.SyncByContent(fullProperties.Names, album);
					SessionHelper.Sync(session, nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(album.WebLinks, fullProperties.WebLinks, album);
					SessionHelper.Sync(session, webLinkDiff);
				}

				// PVs
				if (fullProperties.PVs != null) {

					var pvDiff = CollectionHelper.Diff(album.PVs, fullProperties.PVs, (p1, p2) => (p1.PVId == p2.PVId && p1.Service == p2.Service));

					foreach (var pv in pvDiff.Added) {
						session.Save(album.CreatePV(new PVContract(pv)));
					}

					foreach (var pv in pvDiff.Removed) {
						pv.OnDelete();
						session.Delete(pv);
					}

				}

				album.UpdateArtistString();
				album.UpdateRatingTotals();

				Archive(session, album, AlbumArchiveReason.Reverted, string.Format("Reverted to version {0}", archivedVersion.Version));
				AuditLog(string.Format("reverted {0} to revision {1}", EntryLinkFactory.CreateEntryLink(album), archivedVersion.Version), session);

				return new EntryRevertedContract(album, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int albumId, string[] tags) {

			ParamIs.NotNull(() => tags);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				tags = tags.Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("tagging {0} with {1}", 
					EntryLinkFactory.CreateEntryLink(album), string.Join(", ", tags)), session, user);

				var existingTags = TagHelpers.GetTags(session, tags);

				album.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new AlbumTagUsageFactory(session, album));

				return album.Tags.Usages.OrderByDescending(u => u.Count).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public void UpdateAllReleaseEventNames(string old, string newName) {

			ParamIs.NotNullOrWhiteSpace(() => old);
			ParamIs.NotNullOrWhiteSpace(() => newName);

			old = old.Trim();
			newName = newName.Trim();

			if (old.Equals(newName))
				return;

			VerifyManageDatabase();

			HandleTransaction(session => {

				AuditLog("replacing release event name '" + old + "' with '" + newName + "'", session);

				var albums = session.Query<Album>().Where(a => a.OriginalRelease.EventName == old).ToArray();

				foreach (var a in albums) {
					a.OriginalRelease.EventName = newName;
					NHibernateUtil.Initialize(a.CoverPictureData);
					session.Update(a);
				}

			});

		}

		public void UpdateArtistForAlbumIsSupport(int artistForAlbumId, bool isSupport) {

			VerifyManageDatabase();

			HandleTransaction(session => {
				
				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
				var album = artistForAlbum.Album;

				artistForAlbum.IsSupport = isSupport;
				album.UpdateArtistString();

				AuditLog(string.Format("updated IsSupport for {0} on {1}", artistForAlbum.ArtistToStringOrName, CreateEntryLink(album)), session);

				session.Update(album);

			});

		}

		public void UpdateArtistForAlbumRoles(int artistForAlbumId, ArtistRoles roles) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
				var album = artistForAlbum.Album;

				artistForAlbum.Roles = roles;
				album.UpdateArtistString();

				AuditLog(string.Format("updated roles for {0} on {1}", artistForAlbum.ArtistToStringOrName, CreateEntryLink(album)), session);

				session.Update(album);

			});

		}

	}

	public class AlbumTagUsageFactory : ITagUsageFactory<AlbumTagUsage> {

		private readonly Album album;
		private readonly ISession session;

		public AlbumTagUsageFactory(ISession session, Album album) {
			this.session = session;
			this.album = album;
		}

		public AlbumTagUsage CreateTagUsage(Tag tag) {

			var usage = new AlbumTagUsage(album, tag);
			session.Save(usage);

			return usage;

		}

	}

	public enum AlbumSortRule {

		None,

		Name,

		/// <summary>
		/// By release date in descending order, excluding entries without a full release date.
		/// </summary>
		ReleaseDate,

		/// <summary>
		/// By release date in descending order, including entries without a release date.
		/// Null release dates will be shown LAST (in descending order - in ascending order they'd be shown first).
		/// </summary>
		ReleaseDateWithNulls,

		AdditionDate,

		RatingAverage,

		RatingTotal,

		NameThenReleaseDate,

		CollectionCount

	}

}
