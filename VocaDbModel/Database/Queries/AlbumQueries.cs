using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Database queries related to <see cref="Album"/>.
	/// </summary>
	public class AlbumQueries : QueriesBase<IAlbumRepository, Album> {

		private readonly ObjectCache cache;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IAggregatedEntryImageUrlFactory imageUrlFactory;
		private readonly IEnumTranslations enumTranslations;
		private readonly IFollowedArtistNotifier followedArtistNotifier;
		private readonly IEntryThumbPersister imagePersister;
		private readonly IEntryPictureFilePersister pictureFilePersister;
		private readonly IUserMessageMailer mailer;
		private readonly IPVParser pvParser;
		private readonly IUserIconFactory userIconFactory;

		private IEntryLinkFactory EntryLinkFactory => entryLinkFactory;

		private ArchivedSongVersion ArchiveSong(IDatabaseContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedSongVersion>().Save(archived);
			return archived;

		}

		private Artist[] GetArtists(IDatabaseContext<Album> ctx, ArtistContract[] artistContracts) {
			var ids = artistContracts.Select(a => a.Id).ToArray();
			return ctx.OfType<Artist>().Query().Where(a => ids.Contains(a.Id)).ToArray();			
		}

		private AlbumMergeRecord GetMergeRecord(IDatabaseContext session, int sourceId) {
			return session.Query<AlbumMergeRecord>().FirstOrDefault(s => s.Source == sourceId);
		}

		/// <summary>
		/// Stats shared for all users. These are cached for 1 hour.
		/// </summary>
		private SharedAlbumStatsContract GetSharedAlbumStats(IDatabaseContext ctx, Album album) {

			var key = string.Format("AlbumQueries.SharedAlbumStatsContract.{0}", album.Id);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {

				var latestReview = album.LastReview;
				var latestRatingScore = latestReview != null ? album.UserCollections.FirstOrDefault(uc => uc.User.Equals(latestReview.User)) : null;

				return new SharedAlbumStatsContract {
					ReviewCount = album.Reviews.Count,
					LatestReview = latestReview != null ? new AlbumReviewContract(latestReview, userIconFactory) : null,
					LatestReviewRatingScore = latestRatingScore?.Rating ?? 0,
					OwnedCount = album.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Owned),
					WishlistCount = album.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Wishlisted),
				};
			});

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

		private void UpdateSongArtists(IDatabaseContext<Album> ctx, Song song, ArtistContract[] artistContracts) {

			var artistDiff = song.SyncArtists(artistContracts, 
				addedArtistContracts => GetArtists(ctx, addedArtistContracts));

			ctx.Sync(artistDiff);

			if (artistDiff.Changed) {

				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(PermissionContext)));
				diff.Artists.Set();

				song.UpdateArtistString();
				var archived = ArchiveSong(ctx.OfType<Song>(), song, diff, SongArchiveReason.PropertiesUpdated);
				ctx.Update(song);

				ctx.AuditLogger.AuditLog("updated artists for " + entryLinkFactory.CreateEntryLink(song));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Updated, archived);

			}
			
		}

		public AlbumQueries(IAlbumRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IEntryThumbPersister imagePersister, IEntryPictureFilePersister pictureFilePersister, IUserMessageMailer mailer, 
			IUserIconFactory userIconFactory, IEnumTranslations enumTranslations, IPVParser pvParser,
			IFollowedArtistNotifier followedArtistNotifier, IAggregatedEntryImageUrlFactory entryThumbPersister, ObjectCache cache)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.pictureFilePersister = pictureFilePersister;
			this.mailer = mailer;
			this.userIconFactory = userIconFactory;
			this.enumTranslations = enumTranslations;
			this.pvParser = pvParser;
			this.followedArtistNotifier = followedArtistNotifier;
			this.imageUrlFactory = entryThumbPersister;
			this.cache = cache;

		}

		public AlbumReviewContract AddReview(int albumId, AlbumReviewContract contract) {

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			return HandleTransaction(ctx => {

				AlbumReview review = null;

				if (contract.Id != 0) {
					review = ctx.Load<AlbumReview>(contract.Id);
					if (!review.User.Equals(PermissionContext.LoggedUser)) {
						PermissionContext.VerifyPermission(PermissionToken.DeleteComments);
					}
				} else {
					review = ctx.Query<AlbumReview>().FirstOrDefault(r => r.Album.Id == albumId && r.User.Id == PermissionContext.LoggedUserId && r.LanguageCode == contract.LanguageCode);
				}

				// Create
				if (review == null) {
					var album = ctx.Load<Album>(albumId);
					review = new AlbumReview(album, ctx.OfType<User>().GetLoggedUser(PermissionContext), contract.Title, contract.Text, contract.LanguageCode);					
					album.Reviews.Add(review);
					ctx.Save(review);
				} else { // Update
					review.LanguageCode = contract.LanguageCode;
					review.Text = contract.Text;
					review.Title = contract.Title;
					ctx.Update(review);
				}

				ctx.AuditLogger.AuditLog(string.Format("submitted review for {0}", entryLinkFactory.CreateEntryLink(review.Album)));

				return new AlbumReviewContract(review, userIconFactory);

			});

		}

		public void DeleteReview(int reviewId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			repository.HandleTransaction(ctx => {

				var review = ctx.Load<AlbumReview>(reviewId);

				if (!review.User.Equals(PermissionContext.LoggedUser)) {
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);
				}

				review.Album.Reviews.Remove(review);
				ctx.Delete(review);

			});

		}

		public async Task<IEnumerable<AlbumReviewContract>> GetReviews(int albumId, string languageCode) {

			return await repository.HandleQueryAsync(async ctx => {

				var album = await ctx.LoadAsync(albumId);

				return album.Reviews
					.Where(review => string.IsNullOrEmpty(languageCode) || review.LanguageCode == languageCode)
					.OrderBy(review => review.Date)
					.Select(review => new AlbumReviewContract(review, userIconFactory))
					.ToArray();

			});

		}

		public async Task<IEnumerable<AlbumForUserForApiContract>> GetUserCollections(int albumId, ContentLanguagePreference languagePreference) {

			return await repository.HandleQueryAsync(async ctx => {

				var album = await ctx.LoadAsync(albumId);

				return album.UserCollections
					.Select(uc => new AlbumForUserForApiContract(uc, languagePreference, imageUrlFactory, AlbumOptionalFields.None,
						uc.User.Id == PermissionContext.LoggedUserId || uc.User.Options.PublicAlbumCollection, 
						uc.User.Id == PermissionContext.LoggedUserId || uc.User.Options.PublicAlbumCollection))
					.ToArray();

			});

		}

		public ArchivedAlbumVersion Archive(IDatabaseContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedAlbumVersion>().Save(archived);
			return archived;

		}

		public async Task<ArchivedAlbumVersion> ArchiveAsync(IDatabaseContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			await ctx.SaveAsync(archived);
			return archived;

		}

		public ArchivedAlbumVersion Archive(IDatabaseContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "") {
			return Archive(ctx, album, new AlbumDiff(), reason, notes);
		}

		public async Task<ArchivedAlbumVersion> ArchiveAsync(IDatabaseContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "") {
			return await ArchiveAsync(ctx, album, new AlbumDiff(), reason, notes);
		}

		public ICommentQueries Comments(IDatabaseContext<Album> ctx) {
			return new CommentQueries<AlbumComment, Album>(ctx.OfType<AlbumComment>(), PermissionContext, userIconFactory, entryLinkFactory);
		}

		public async Task<AlbumContract> Create(CreateAlbumContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			VerifyManageDatabase();

			return await repository.HandleTransactionAsync(async ctx => {

				ctx.AuditLogger.SysLog(string.Format("creating a new album with name '{0}'", contract.Names.First().Value));

				var album = new Album { DiscType = contract.DiscType };

				album.Names.Init(contract.Names, album);

				await ctx.SaveAsync(album);

				foreach (var artistContract in contract.Artists) {
					var artist = await ctx.LoadAsync<Artist>(artistContract.Id);
					if (!album.HasArtist(artist)) {
						await ctx.SaveAsync(artist.AddAlbum(album));
					}
				}

				album.UpdateArtistString();
				var archived = await ArchiveAsync(ctx, album, AlbumArchiveReason.Created);
				await ctx.UpdateAsync(album);

				await ctx.AuditLogger.AuditLogAsync(string.Format("created album {0} ({1})", entryLinkFactory.CreateEntryLink(album), album.DiscType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), album, EntryEditEvent.Created, archived);

				await followedArtistNotifier.SendNotificationsAsync(ctx, album, album.ArtistList, PermissionContext.LoggedUser);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public CommentForApiContract CreateComment(int albumId, CommentForApiContract contract) {

			return HandleTransaction(ctx => Comments(ctx).Create(albumId, contract));

		}

		public (bool created, int reportId) CreateReport(int albumId, AlbumReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory,
					(album, reporter, notesTruncated) => new AlbumReport(album, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != AlbumReportType.Other ? enumTranslations.AlbumReportTypeNames[reportType] : null,
					albumId, reportType, hostname, notes);
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
						CommentCount = a.Comments.Count,
						Hits = a.Hits.Count,
					})
					.FirstOrDefault();

				if (stats == null)
					throw new ObjectNotFoundException(id, typeof(Album));

				var user = PermissionContext.LoggedUser;

				SongVoteRating? GetRatingFunc(Song song) {
					return user != null && song != null ? (SongVoteRating?) session.Query<FavoriteSongForUser>().Where(s => s.Song.Id == song.Id && s.User.Id == user.Id).Select(r => r.Rating).FirstOrDefault() : null;
				}

				var contract = new AlbumDetailsContract(album, PermissionContext.LanguagePreference, PermissionContext, imageUrlFactory, GetRatingFunc,
					discTypeTag: new EntryTypeTags(session).GetTag(EntryType.Album, album.DiscType)) {
					CommentCount = stats.CommentCount,
					Hits = stats.Hits,
					Stats = GetSharedAlbumStats(session, album)
				};

				if (user != null) {

					var albumForUser = session.Query<AlbumForUser>()
						.FirstOrDefault(a => a.Album.Id == id && a.User.Id == user.Id);

					contract.AlbumForUser = (albumForUser != null ? new AlbumForUserContract(albumForUser, PermissionContext.LanguagePreference) : null);

				}

				contract.LatestComments = session.Query<AlbumComment>()
					.Where(c => c.EntryForComment.Id == id)
					.OrderByDescending(c => c.Created)
					.Take(3)
					.ToArray()
					.Select(c => new CommentForApiContract(c, userIconFactory))
					.ToArray();

				if (album.Deleted) {
					var mergeEntry = GetMergeRecord(session, id);
					contract.MergedTo = (mergeEntry != null ? new AlbumContract(mergeEntry.Target, LanguagePreference) : null);
				}

				if (user != null || !string.IsNullOrEmpty(hostname)) {

					var agentNum = (user != null ? user.Id : hostname.GetHashCode());

					using (var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted)) {

						var isHit = session.Query<AlbumHit>().Any(h => h.Entry.Id == id && h.Agent == agentNum);

						if (!isHit) {

							var hit = new AlbumHit(album, agentNum);
							session.Save(hit);

							try {
								tx.Commit();
							} catch (SqlException x) {
								session.AuditLogger.SysLog("Error while committing hit: " + x.Message);
							}

						}

					}

				}

				return contract;

			});

		}

		public T GetAlbumWithMergeRecord<T>(int id, Func<Album, AlbumMergeRecord, T> fac) {

			return HandleQuery(session => {
				var album = session.Load<Album>(id);
				return fac(album, (album.Deleted ? GetMergeRecord(session, id) : null));
			});

		}

		public CommentForApiContract[] GetComments(int albumId) {
			
			return HandleQuery(ctx => Comments(ctx).GetAll(albumId));

		}

		public EntryForPictureDisplayContract GetCoverPictureThumb(int albumId) {
			
			var size = ImageSize.Thumb;

			// TODO: this all should be moved to DynamicImageUrlFactory
			return repository.HandleQuery(ctx => {
				
				var album = ctx.Load(albumId);

				// If there is no picture, return empty.
				if (album.CoverPictureData == null || string.IsNullOrEmpty(album.CoverPictureMime))
					return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference);

				// Try to read thumbnail from file system.
				var data = album.Thumb;
				if (imagePersister.HasImage(data, size)) {
					var bytes = imagePersister.ReadBytes(data, size);
					return EntryForPictureDisplayContract.Create(album, data.Mime, bytes, PermissionContext.LanguagePreference);
				}

				// This should return the original image.
				return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference);

			});

		}

		public AlbumForEditContract GetForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), PermissionContext.LanguagePreference, imageUrlFactory));

		}

		public RelatedAlbumsContract GetRelatedAlbums(int albumId) {

			return repository.HandleQuery(ctx => {

				var album = ctx.Load(albumId);
				var q = new RelatedAlbumsQuery(ctx);
				var albums = q.GetRelatedAlbums(album);

				return new RelatedAlbumsContract { 
					ArtistMatches = 
						albums.ArtistMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray(),
					LikeMatches = 
						albums.LikeMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray(),
					TagMatches = 
						albums.TagMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name)
						.ToArray()
					};

			});

		}

		public Task<TagUsageForApiContract[]> GetTagSuggestions(int albumId) {

			var maxResults = 3;

			return repository.HandleQueryAsync(async ctx => {

				var album = ctx.Load<Album>(albumId);
				var albumTags = album.Tags.Tags.Select(t => t.Id);

				var songUsages = ctx.Query<SongTagUsage>()
					.Where(u => !albumTags.Contains(u.Tag.Id)
						&& !u.Tag.Deleted
						&& !u.Tag.HideFromSuggestions
						&& u.Entry.AllAlbums.Any(a => a.Album.Id == albumId))
					.WhereTagHasTarget(TagTargetTypes.Album)
					.GroupBy(t => t.Tag.Id)
					.Select(t => new { TagId = t.Key, Count = t.Count() })
					.Where(t => t.Count > 1)
					.OrderByDescending(t => t.Count)
					.Take(maxResults)
					.ToArray();

				var tags = ctx.LoadMultiple<Tag>(songUsages.Select(t => t.TagId)).ToDictionary(t => t.Id);

				var results = songUsages.Select(t => new TagUsageForApiContract(tags[t.TagId], t.Count, LanguagePreference));

				if (songUsages.Length < 3) {

					var pvResults = await pvParser.ParseByUrlsAsync(album.PVs
						.Where(pv => pv.Service == PVService.NicoNicoDouga)
						.Select(pv => pv.Url), true, permissionContext);

					var nicoTags = pvResults.SelectMany(pv => pv.Tags).Distinct().ToArray();
					var mappedTags = new TagMapper().MapTags(ctx, nicoTags)
						.Where(tag => !albumTags.Contains(tag.Id) && !tags.ContainsKey(tag.Id));

					results = results
						.Concat(mappedTags.Select(tag => new TagUsageForApiContract(tag, 0, LanguagePreference)))
						.Take(maxResults);

				}

				return results.ToArray();

			});

		}

		public IEnumerable<Dictionary<string, string>> GetTracksFormatted(int id, int? discNumber, string[] fields, ContentLanguagePreference lang) {

			if (fields == null || fields.Length == 0)
				fields = new[] { "id", "title" };

			return HandleQuery(db => new AlbumSongFormatter(entryLinkFactory).ApplyFormatDict(db.Load(id), fields, discNumber, lang));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target albums can't be the same", "targetId");

			repository.HandleTransaction(session => {

				var source = session.Load(sourceId);
				var target = session.Load(targetId);

				session.AuditLogger.AuditLog(string.Format("Merging {0} to {1}", EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)));

				NHibernateUtil.Initialize(source.CoverPictureData);
				NHibernateUtil.Initialize(target.CoverPictureData);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					session.Save(link);
				}

				var artists = source.Artists.Where(a => !target.HasArtistForAlbum(a)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => s.Song == null || !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
				}

				var pictures = source.Pictures.ToArray();
				foreach (var p in pictures) {
					p.Move(target);
					session.Update(p);
				}

				var userCollections = source.UserCollections.Where(a => !target.IsInUserCollection(a.User)).ToArray();
				foreach (var u in userCollections) {
					u.Move(target);
					session.Update(u);
				}

				target.Description.CopyIfEmpty(source.Description);

				if (target.OriginalRelease == null)
					target.OriginalRelease = new AlbumRelease();

				if (string.IsNullOrEmpty(target.OriginalRelease.CatNum) && source.OriginalRelease != null)
					target.OriginalRelease.CatNum = source.OriginalRelease.CatNum;

				if (target.OriginalRelease.ReleaseEvent == null && source.OriginalRelease != null)
					target.OriginalRelease.ReleaseEvent = source.OriginalRelease.ReleaseEvent;

				if (target.OriginalRelease.ReleaseDate == null)
					target.OriginalRelease.ReleaseDate = new OptionalDateTime();

				if (target.OriginalReleaseDate.Year == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Year = source.OriginalReleaseDate.Year;

				if (target.OriginalReleaseDate.Month == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Month = source.OriginalReleaseDate.Month;

				if (target.OriginalReleaseDate.Day == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Day = source.OriginalReleaseDate.Day;

				// Create merge record
				var mergeEntry = new AlbumMergeRecord(source, target);
				session.Save(mergeEntry);

				source.Deleted = true;

				target.UpdateArtistString();
				target.Names.UpdateSortNames();

				Archive(session, source, AlbumArchiveReason.Deleted, string.Format("Merged to {0}", target));
				Archive(session, target, AlbumArchiveReason.Merged, string.Format("Merged from {0}", source));

				session.Update(source);
				session.Update(target);

			});

		}

		public int MoveToTrash(int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			return HandleTransaction(ctx => {

				var album = ctx.Load<Album>(albumId);

				AuditLog(string.Format("moving {0} to trash", album), ctx);

				NHibernateUtil.Initialize(album.CoverPictureData);

				var archived = new ArchivedAlbumContract(album, new AlbumDiff(true));
				var data = XmlHelper.SerializeToXml(archived);
				var trashed = new TrashedEntry(album, data, GetLoggedUser(ctx));

				ctx.Save(trashed);

				album.DeleteLinks();

				var ctxActivity = ctx.OfType<AlbumActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(t => t.Entry.Id == albumId).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.Delete(album);

				return trashed.Id;

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return new TagUsageQueries(PermissionContext).RemoveTagUsage<AlbumTagUsage, Album>(tagUsageId, repository);

		}

		public EntryRevertedContract RevertToVersion(int archivedAlbumVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedAlbumVersion>(archivedAlbumVersionId);
				var album = archivedVersion.Album;

				session.AuditLogger.SysLog("reverting " + album + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedAlbumContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();
				var diff = new AlbumDiff();

				album.Description.Original = fullProperties.Description;
				album.Description.English = fullProperties.DescriptionEng ?? string.Empty;
				album.DiscType = fullProperties.DiscType;
				album.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);

				if (versionWithPic != null) {

					album.CoverPictureData = versionWithPic.CoverPicture;
					album.CoverPictureMime = versionWithPic.CoverPictureMime;

					if (versionWithPic.CoverPicture != null) {

						var thumbGenerator = new ImageThumbGenerator(imagePersister);
						using (var stream = new MemoryStream(versionWithPic.CoverPicture.Bytes)) {
							var thumb = new EntryThumb(album, versionWithPic.CoverPictureMime, ImagePurpose.Main);
							thumbGenerator.GenerateThumbsAndMoveImage(stream, thumb, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);
						}

					}


				} else {

					album.CoverPictureData = null;
					album.CoverPictureMime = null;

				}

				// Assume picture was changed if there's a version between the current version and the restored version where the picture was changed.
				diff.Cover.Set(!Equals(album.ArchivedVersionsManager.GetLatestVersionWithField(AlbumEditableFields.Cover, album.Version), versionWithPic));

				// Original release
				album.OriginalRelease = (fullProperties.OriginalRelease != null ? new AlbumRelease(fullProperties.OriginalRelease, session.NullSafeLoad<ReleaseEvent>(fullProperties.OriginalRelease.ReleaseEvent)) : null);

				// Artists
				DatabaseContextHelper.RestoreObjectRefs<ArtistForAlbum, Artist, ArchivedArtistForAlbumContract>(
					session.OfType<Artist>(), warnings, album.AllArtists, fullProperties.Artists,
					(a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(artist, albumRef) => RestoreArtistRef(album, artist, albumRef),
					albumForArtist => albumForArtist.Delete());

				// Songs
				DatabaseContextHelper.RestoreObjectRefs<SongInAlbum, Song, SongInAlbumRefContract>(
					session.OfType<Song>(), warnings, album.AllSongs, fullProperties.Songs,
					(a1, a2) => ((a1.Song != null && a1.Song.Id == a2.Id) || a1.Song == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(song, songRef) => RestoreTrackRef(album, song, songRef),
					songInAlbum => songInAlbum.Delete());

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = album.Names.SyncByContent(fullProperties.Names, album);
					session.Sync(nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(album.WebLinks, fullProperties.WebLinks, album);
					session.Sync(webLinkDiff);
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

				Archive(session, album, diff, AlbumArchiveReason.Reverted, string.Format("Reverted to version {0}", archivedVersion.Version));
				AuditLog(string.Format("reverted {0} to revision {1}", EntryLinkFactory.CreateEntryLink(album), archivedVersion.Version), session);

				return new EntryRevertedContract(album, warnings);

			});

		}

		public async Task<AlbumForEditContract> UpdateBasicProperties(AlbumForEditContract properties, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => properties);

			return await repository.HandleTransactionAsync(async session => {

				var album = session.Load(properties.Id);

				VerifyEntryEdit(album);

				var diff = new AlbumDiff(DoSnapshot(album.ArchivedVersionsManager.GetLatestVersion(), session.OfType<User>().GetLoggedUser(PermissionContext)));

				session.AuditLogger.SysLog(string.Format("updating properties for {0}", album));

				if (album.DiscType != properties.DiscType) {
					album.DiscType = properties.DiscType;
					album.UpdateArtistString();
					diff.DiscType.Set();
				}

				diff.Description.Set(album.Description.CopyFrom(properties.Description));

				var parsedBarcodes = properties.Identifiers.Select(Album.ParseBarcode).ToArray();
				var barcodeDiff = album.SyncIdentifiers(parsedBarcodes);
				session.Sync(barcodeDiff);
				if (barcodeDiff.Changed) {
					diff.Identifiers.Set();
				}

				if (album.TranslatedName.DefaultLanguage != properties.DefaultNameLanguage) {
					album.TranslatedName.DefaultLanguage = properties.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var validNames = properties.Names;
				var nameDiff = album.Names.Sync(validNames, album);
				session.OfType<AlbumName>().Sync(nameDiff);

				album.Names.UpdateSortNames();

				if (nameDiff.Changed)
					diff.Names.Set();

				var webLinkDiff = WebLink.Sync(album.WebLinks, properties.WebLinks, album);
				session.OfType<AlbumWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks.Set();

				var newEvent = new CreateEventQuery().FindOrCreate(session, PermissionContext, properties.OriginalRelease.ReleaseEvent, album);
				var newOriginalRelease = (properties.OriginalRelease != null ? new AlbumRelease(properties.OriginalRelease, newEvent) : new AlbumRelease());

				if (album.OriginalRelease == null)
					album.OriginalRelease = new AlbumRelease();

				if (!album.OriginalRelease.Equals(newOriginalRelease)) {
					album.OriginalRelease = newOriginalRelease;
					diff.OriginalRelease.Set();
				}

				// Required because of a bug in NHibernate
				NHibernateUtil.Initialize(album.CoverPictureData);

				if (pictureData != null) {

					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					album.CoverPictureData = new PictureData(parsed);
					album.CoverPictureMime = parsed.Mime;

					pictureData.Id = album.Id;
					pictureData.EntryType = EntryType.Album;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.AllThumbs);

					diff.Cover.Set();

				}

				if (album.Status != properties.Status) {
					album.Status = properties.Status;
					diff.Status.Set();
				}

				var artistGetter = new Func<ArtistContract, Artist>(artist => 
					session.OfType<Artist>().Load(artist.Id));

				var artistsDiff = album.SyncArtists(properties.ArtistLinks, artistGetter);
				session.OfType<ArtistForAlbum>().Sync(artistsDiff);

				if (artistsDiff.Changed)
					diff.Artists.Set();

				var discsDiff = album.SyncDiscs(properties.Discs);
				session.OfType<AlbumDiscProperties>().Sync(discsDiff);

				if (discsDiff.Changed)
					diff.Discs.Set();

				var songGetter = new Func<SongInAlbumEditContract, Song>(contract => {

					if (contract.SongId != 0)
						return session.Load<Album, Song>(contract.SongId);
					else {

						var songName = StringHelper.TrimIfNotWhitespace(contract.SongName);

						session.AuditLogger.SysLog(string.Format("creating a new song '{0}' to {1}", songName, album));

						var song = new Song(new LocalizedString(songName, ContentLanguageSelection.Unspecified));
						session.Save(song);

						var songDiff = new SongDiff();
						songDiff.Names.Set();
						var songArtistDiff = song.SyncArtists(contract.Artists, 
							addedArtistContracts => GetArtists(session, addedArtistContracts));

						if (songArtistDiff.Changed) {
							songDiff.Artists.Set();
							session.Update(song);
						}

						session.Sync(songArtistDiff);

						var archived = ArchiveSong(session.OfType<Song>(), song, songDiff, SongArchiveReason.Created,
							string.Format("Created for album '{0}'", album.DefaultName.TruncateWithEllipsis(100)));

						session.AuditLogger.AuditLog(string.Format("created {0} for {1}",
							entryLinkFactory.CreateEntryLink(song), entryLinkFactory.CreateEntryLink(album)));
						AddEntryEditedEntry(session.OfType<ActivityEntry>(), song, EntryEditEvent.Created, archived);

						return song;

					}

				});

				var tracksDiff = album.SyncSongs(properties.Songs, songGetter, 
					(song, artistContracts) => UpdateSongArtists(session, song, artistContracts));

				session.OfType<SongInAlbum>().Sync(tracksDiff);

				if (tracksDiff.Changed) {

					var add = string.Join(", ", tracksDiff.Added.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));
					var rem = string.Join(", ", tracksDiff.Removed.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));
					var edit = string.Join(", ", tracksDiff.Edited.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));

					var str = string.Format("edited tracks (added: {0}, removed: {1}, reordered: {2})", add, rem, edit)
						.Truncate(300);

					session.AuditLogger.AuditLog(str);

					diff.Tracks.Set();

				}

				var picsDiff = album.Pictures.SyncPictures(properties.Pictures, session.OfType<User>().GetLoggedUser(PermissionContext), album.CreatePicture);
				session.OfType<AlbumPictureFile>().Sync(picsDiff);
				var entryPictureFileThumbGenerator = new ImageThumbGenerator(pictureFilePersister);
				album.Pictures.GenerateThumbsAndMoveImage(entryPictureFileThumbGenerator, picsDiff.Added, ImageSizes.Original | ImageSizes.Thumb);

				if (picsDiff.Changed)
					diff.Pictures.Set();

				var pvDiff = album.SyncPVs(properties.PVs);
				session.OfType<PVForAlbum>().Sync(pvDiff);

				if (pvDiff.Changed)
					diff.PVs.Set();

				var logStr = string.Format("updated properties for album {0} ({1})", 
					entryLinkFactory.CreateEntryLink(album), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				session.AuditLogger.AuditLog(logStr);

				var archivedAlbum = Archive(session, album, diff, AlbumArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				session.Update(album);

				AddEntryEditedEntry(session.OfType<ActivityEntry>(), album, EntryEditEvent.Updated, archivedAlbum);

				var newSongCutoff = TimeSpan.FromHours(1);
				if (artistsDiff.Added.Any() && album.CreateDate >= DateTime.Now - newSongCutoff) {

					var addedArtists = artistsDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

					if (addedArtists.Any()) {
						await followedArtistNotifier.SendNotificationsAsync(session, album, addedArtists, PermissionContext.LoggedUser);
					}

				}

				return new AlbumForEditContract(album, PermissionContext.LanguagePreference, imageUrlFactory);

			});

		}

		public void UpdatePersonalDescription(int albumId, AlbumDetailsContract data) {

			PermissionContext.VerifyLogin();

			HandleTransaction(ctx => {

				var album = ctx.Load(albumId);

				EntryPermissionManager.VerifyAccess(PermissionContext, album, EntryPermissionManager.CanEditPersonalDescription);

				album.PersonalDescriptionText = data.PersonalDescriptionText;
				album.PersonalDescriptionAuthorId = data.PersonalDescriptionAuthor?.Id;

				ctx.Update(album);
				ctx.AuditLogger.AuditLog(string.Format("updated personal description for {0}", entryLinkFactory.CreateEntryLink(album)));

			});

		}

	}
}