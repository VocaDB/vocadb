using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using NLog;
using VocaDb.Model.Domain.Globalization;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using System.Drawing;
using VocaDb.Model.Helpers;
using System.Collections.Generic;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
// ReSharper restore UnusedMember.Local

		public PartialFindResult<Artist> Find(ISession session, ArtistQueryParams queryParams) {

			var context = new NHibernateRepositoryContext<Artist>(session, PermissionContext);
			return new ArtistSearch(LanguagePreference, context).Find(queryParams);

		}

		private ArtistMergeRecord GetMergeRecord(ISession session, int sourceId) {
			return session.Query<ArtistMergeRecord>().FirstOrDefault(s => s.Source == sourceId);
		}

		public ArtistService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public ArtistForAlbumContract AddAlbum(int artistId, int albumId) {

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var album = session.Load<Album>(albumId);

				var hasAlbum = session.Query<ArtistForAlbum>().Any(a => a.Artist.Id == artistId && a.Album.Id == albumId);

				if (hasAlbum)
					throw new LinkAlreadyExistsException(string.Format("{0} already has {1}", artist, album));

				AuditLog(string.Format("adding {0} for {1}", 
					EntryLinkFactory.CreateEntryLink(album), EntryLinkFactory.CreateEntryLink(artist)), session);

				var artistForAlbum = artist.AddAlbum(album);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public void Archive(ISession session, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "") {

			SysLog("Archiving " + artist);

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Artist artist, ArtistArchiveReason reason, string notes = "") {

			Archive(session, artist, new ArtistDiff(), reason, notes);

		}

		public bool CreateReport(int artistId, ArtistReportType reportType, string hostname, string notes) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(session => {

				var loggedUserId = PermissionContext.LoggedUserId;
				var existing = session.Query<ArtistReport>()
					.FirstOrDefault(r => r.Artist.Id == artistId
						&& ((loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname));

				if (existing != null)
					return false;

				var artist = session.Load<Artist>(artistId);
				var report = new ArtistReport(artist, reportType, GetLoggedUserOrDefault(session), hostname, notes.Truncate(EntryReport.MaxNotesLength));

				var msg = string.Format("reported {0} as {1} ({2})", EntryLinkFactory.CreateEntryLink(artist), reportType, HttpUtility.HtmlEncode(notes));
				AuditLog(msg.Truncate(200), session, new AgentLoginData(GetLoggedUserOrDefault(session), hostname));

				session.Save(report);
				return true;

			}, IsolationLevel.ReadUncommitted);

		}

		public void Delete(int id) {

			UpdateEntity<Artist>(id, (session, a) => {

				AuditLog(string.Format("deleting artist {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				NHibernateUtil.Initialize(a.Picture);
				a.Delete();
			                         
			}, PermissionToken.DeleteEntries, skipLog: true);

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<ArtistComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.Artist.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		public PartialFindResult<ArtistContract> FindArtists(ArtistQueryParams queryParams) {

			return FindArtists(a => new ArtistContract(a, PermissionContext.LanguagePreference), queryParams);

		}

		public PartialFindResult<T> FindArtists<T>(Func<Artist, T> fac, ArtistQueryParams queryParams) {

			return HandleQuery(session => {

				var result = Find(session, queryParams);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
					result.TotalCount, result.Term, result.FoundExactMatch);

			});

		}

		public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName, string url) {

			var names = anyName.Select(n => n.Trim()).Where(n => n != string.Empty).ToArray();
			var urlTrimmed = url != null ? url.Trim() : null;

			if (!names.Any() && string.IsNullOrEmpty(url))
				return new EntryRefWithCommonPropertiesContract[] { };

			return HandleQuery(session => {

				// TODO: moved Distinct after ToArray to work around NH bug
				var nameMatches = (names.Any() ? session.Query<ArtistName>()
					.Where(n => names.Contains(n.Value) && !n.Artist.Deleted)
					.OrderBy(n => n.Artist)
					.Select(n => n.Artist)
					.Take(10)
					.ToArray()
					.Distinct(): new Artist[] {});

				var linkMatches = !string.IsNullOrEmpty(urlTrimmed) ?
					session.Query<ArtistWebLink>()
					.Where(w => w.Url == urlTrimmed)
					.Select(w => w.Artist)
					.Take(10)
					.ToArray()
					.Distinct() : new Artist[] {};

				return nameMatches.Union(linkMatches)
					.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public string[] FindNames(string query, int maxResults) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] {};

			var matchMode = NameMatchMode.Auto;
			query = FindHelpers.GetMatchModeAndQueryForSearch(query, ref matchMode);

			return HandleQuery(session => {

				var names = session.Query<ArtistName>()
					.Where(a => !a.Artist.Deleted)
					.FilterByArtistName(query, matchMode: matchMode)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(names, query);

			});

		}

		public EntryForPictureDisplayContract GetArchivedArtistPicture(int archivedVersionId) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(
				session.Load<ArchivedArtistVersion>(archivedVersionId), LanguagePreference));

		}

		public ArtistContract GetArtist(int id) {

			return HandleQuery(session => new ArtistContract(session.Load<Artist>(id), LanguagePreference));

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

		}

		public ArtistContract GetArtistWithAdditionalNames(int id) {

			return HandleQuery(session => new ArtistContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

		}

		/// <summary>
		/// Gets the picture for a <see cref="Artist"/>.
		/// </summary>
		/// <param name="id">Artist Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public EntryForPictureDisplayContract GetArtistPicture(int id, Size requestedSize) {

			return HandleQuery(session => 
				EntryForPictureDisplayContract.Create(session.Load<Artist>(id), PermissionContext.LanguagePreference, requestedSize));

		}

		public ArtistWithArchivedVersionsContract GetArtistWithArchivedVersions(int artistId) {

			return HandleQuery(session => new ArtistWithArchivedVersionsContract(
				session.Load<Artist>(artistId), PermissionContext.LanguagePreference));

		}

		public ArtistForApiContract[] GetArtistsWithYoutubeChannels(ContentLanguagePreference languagePreference) {

			return HandleQuery(session => {

				var contracts = session.Query<ArtistWebLink>()
					.Where(l => !l.Artist.Deleted 
						&& (l.Artist.ArtistType == ArtistType.Producer || l.Artist.ArtistType == ArtistType.Circle || l.Artist.ArtistType == ArtistType.Animator) 
						&& (l.Url.Contains("youtube.com/user/") || l.Url.Contains("youtube.com/channel/")))
					.Select(l => l.Artist)
					.Distinct()
					.ToArray()
					.Select(a => new ArtistForApiContract(a, languagePreference, null, false, ArtistOptionalFields.WebLinks))
					.ToArray();

				return contracts;

			});

		}

		public CommentContract[] GetComments(int artistId) {

			return HandleQuery(session => {

				return session.Query<ArtistComment>()
					.Where(c => c.Artist.Id == artistId)
					.OrderByDescending(c => c.Created)
					.Select(c => new CommentContract(c)).ToArray();

			});

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int artistId) {

			return HandleQuery(session => {

				var artist = session.Load<Artist>(artistId);
				return new EntryWithTagUsagesContract(artist, artist.Tags.Usages);

			});

		}

		public TagSelectionContract[] GetTagSelections(int artistId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<ArtistTagUsage>().Where(a => a.Artist.Id == artistId).ToArray();
				var tagVotes = session.Query<ArtistTagVote>().Where(a => a.User.Id == userId && a.Usage.Artist.Id == artistId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public ArchivedArtistVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedArtistVersionDetailsContract(session.Load<ArchivedArtistVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedArtistVersion>(comparedVersionId) : null, PermissionContext.LanguagePreference));

		}

		public XDocument GetVersionXml(int id) {
			return HandleQuery(session => session.Load<ArchivedArtistVersion>(id).Data);
		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target artists can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Artist>(sourceId);
				var target = session.Load<Artist>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", 
					EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)), session);

				NHibernateUtil.Initialize(source.Picture);
				NHibernateUtil.Initialize(target.Picture);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					session.Save(link);
				}

				var groups = source.Groups.Where(g => !target.HasGroup(g.Group)).ToArray();
				foreach (var g in groups) {
					g.MoveToMember(target);
					session.Update(g);
				}

				var members = source.Members.Where(m => !m.Member.HasGroup(target)).ToArray();
				foreach (var m in members) {
					m.MoveToGroup(target);
					session.Update(m);
				}

				var albums = source.Albums.Where(a => !target.HasAlbum(a.Album)).ToArray();
				foreach (var a in albums) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
				}

				var ownerUsers = source.OwnerUsers.Where(s => !target.HasOwnerUser(s.User)).ToArray();
				foreach (var u in ownerUsers) {
					u.Move(target);
					session.Update(u);
				}

				var pictures = source.Pictures.ToArray();
				foreach (var p in pictures) {
					p.Move(target);
					session.Update(p);
				}

				var users = source.Users.ToArray();
				foreach (var u in users) {
					u.Move(target);
					session.Update(u);
				}

				if (target.Description == string.Empty)
					target.Description = source.Description;

				// Create merge record
				var mergeEntry = new ArtistMergeRecord(source, target);
				session.Save(mergeEntry);

				source.Deleted = true;

				Archive(session, target, ArtistArchiveReason.Merged, string.Format("Merged from '{0}'", source));

				NHibernateUtil.Initialize(source.Picture);
				NHibernateUtil.Initialize(target.Picture);

				session.Update(source);
				session.Update(target);

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return RemoveTagUsage<ArtistTagUsage>(tagUsageId);

		}

		public void Restore(int artistId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);

				NHibernateUtil.Initialize(artist.Picture);
				artist.Deleted = false;

				session.Update(artist);

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(artist), session);

			});

		}

		/// <summary>
		/// Reverts an album to an earlier archived version.
		/// </summary>
		/// <param name="archivedArtistVersionId">Id of the archived version to be restored.</param>
		/// <returns>Result of the revert operation, with possible warnings if any. Cannot be null.</returns>
		/// <remarks>Requires the RestoreRevisions permission.</remarks>
		public EntryRevertedContract RevertToVersion(int archivedArtistVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedArtistVersion>(archivedArtistVersionId);
				var artist = archivedVersion.Artist;

				SysLog("reverting " + artist + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedArtistContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				artist.ArtistType = fullProperties.ArtistType;
				artist.Description = fullProperties.Description;
				artist.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;
				artist.BaseVoicebank = SessionHelper.RestoreWeakRootEntityRef<Artist>(session, warnings, fullProperties.BaseVoicebank);

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);

				if (versionWithPic != null)
					artist.Picture = versionWithPic.Picture;

				/*
				// Albums
				SessionHelper.RestoreObjectRefs<ArtistForAlbum, Album>(
					session, warnings, artist.AllAlbums, fullProperties.Albums, (a1, a2) => (a1.Album.Id == a2.Id),
					album => (!artist.HasAlbum(album) ? artist.AddAlbum(album) : null),
					albumForArtist => albumForArtist.Delete());
				 */

				// Groups
				SessionHelper.RestoreObjectRefs<GroupForArtist, Artist>(
					session, warnings, artist.AllGroups, fullProperties.Groups, (a1, a2) => (a1.Group.Id == a2.Id),
					grp => (!artist.HasGroup(grp) ? artist.AddGroup(grp) : null),
					groupForArtist => groupForArtist.Delete());

				/*
				// Members
				SessionHelper.RestoreObjectRefs<GroupForArtist, Artist>(
					session, warnings, artist.AllMembers, fullProperties.Members, (a1, a2) => (a1.Member.Id == a2.Id),
					member => (!artist.HasMember(member) ? artist.AddMember(member) : null),
					groupForArtist => groupForArtist.Delete());
				 */

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = artist.Names.SyncByContent(fullProperties.Names, artist);
					SessionHelper.Sync(session, nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(artist.WebLinks, fullProperties.WebLinks, artist);
					SessionHelper.Sync(session, webLinkDiff);
				}

				Archive(session, artist, ArtistArchiveReason.Reverted);
				AuditLog(string.Format("reverted {0} to revision {1}", EntryLinkFactory.CreateEntryLink(artist), archivedVersion.Version), session);

				return new EntryRevertedContract(artist, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int artistId, string[] tags) {

			ParamIs.NotNull(() => tags);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				tags = tags.Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var artist = session.Load<Artist>(artistId);

				AuditLog(string.Format("tagging {0} with {1}", 
					EntryLinkFactory.CreateEntryLink(artist), string.Join(", ", tags)), session, user);

				var existingTags = TagHelpers.GetTags(session, tags);

				artist.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new ArtistTagUsageFactory(session, artist));

				return artist.Tags.Usages.OrderByDescending(u => u.Count).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

	}

	public class ArtistTagUsageFactory : ITagUsageFactory<ArtistTagUsage> {

		private readonly Artist artist;
		private readonly ISession session;

		public ArtistTagUsageFactory(ISession session, Artist artist) {
			this.session = session;
			this.artist = artist;
		}

		public ArtistTagUsage CreateTagUsage(Tag tag) {

			var usage = new ArtistTagUsage(artist, tag);
			session.Save(usage);

			return usage;

		}

	}

	public enum ArtistSortRule {

		/// <summary>
		/// Not sorted (random order)
		/// </summary>
		None,

		/// <summary>
		/// Sort by name (ascending)
		/// </summary>
		Name,

		/// <summary>
		/// Sort by addition date (descending)
		/// </summary>
		AdditionDate,

		/// <summary>
		/// Sort by addition date (ascending)
		/// </summary>
		AdditionDateAsc,

		SongCount,

		SongRating

	}

}
