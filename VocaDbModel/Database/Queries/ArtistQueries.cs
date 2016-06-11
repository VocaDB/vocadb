using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
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
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Database queries related to <see cref="Artist"/>.
	/// </summary>
	public class ArtistQueries : QueriesBase<IArtistRepository, Artist> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly ObjectCache cache;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEnumTranslations enumTranslations;
		private readonly IEntryThumbPersister imagePersister;
		private readonly IEntryPictureFilePersister pictureFilePersister;
		private readonly IUserIconFactory userIconFactory;

		class CachedAdvancedArtistStatsContract {
			
			public TopStatContract<TranslatedArtistContract>[] TopVocaloids { get; set; }

		}

		private AdvancedArtistStatsContract GetAdvancedStats(IDatabaseContext<Artist> ctx, Artist artist) {
			
			if (artist.ArtistType != ArtistType.Producer)
				return null;

			var key = string.Format("ArtistQueries.AdvancedArtistStatsContract.{0}", artist.Id);

			var cached = cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(24), () => {

				var types = new[] { ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVoiceSynthesizer };

				var topVocaloidIdsAndCounts = ctx
					.Query<ArtistForSong>()
					.Where(a => types.Contains(a.Artist.ArtistType) && !a.Song.Deleted && a.Song.AllArtists.Any(ar => !ar.IsSupport && ar.Artist.Id == artist.Id))
					.GroupBy(a => a.Artist.Id)
					.Select(a => new {
						ArtistId = a.Key,
						Count = a.Count()
					})
					.OrderByDescending(a => a.Count)
					.Take(3)
					.ToDictionary(a => a.ArtistId, a => a.Count);

				var topVocaloidIds = topVocaloidIdsAndCounts.Select(i => i.Key).ToArray();
					
				var topVocaloids = ctx.Query().Where(a => topVocaloidIds.Contains(a.Id))
					.ToArray()
					.Select(a => new TopStatContract<TranslatedArtistContract> {
						Data = new TranslatedArtistContract(a),
						Count = topVocaloidIdsAndCounts[a.Id]
					})
					.OrderByDescending(d => d.Count)
					.ToArray();

				return new CachedAdvancedArtistStatsContract {
					TopVocaloids = topVocaloids
				};

			});

			return new AdvancedArtistStatsContract {
				TopVocaloids = cached.TopVocaloids.Select(v => new TopStatContract<ArtistContract> {
					Data = new ArtistContract(v.Data, LanguagePreference),
					Count = v.Count
				}).ToArray()
			};

		}

		private PersonalArtistStatsContract GetPersonalArtistStats(IDatabaseContext<Artist> ctx, Artist artist) {
			
			if (!PermissionContext.IsLoggedIn)
				return null;

			var key = string.Format("ArtistQueries.PersonalArtistStatsContract.{0}.{1}", artist.Id, PermissionContext.LoggedUserId);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {
				
				return new PersonalArtistStatsContract {
					SongRatingCount = ctx.OfType<FavoriteSongForUser>()
						.Query()
						.Count(f => f.User.Id == PermissionContext.LoggedUserId && f.Song.AllArtists.Any(a => a.Artist.Id == artist.Id))
				};

			});
			
		}

		private SharedArtistStatsContract GetSharedArtistStats(IDatabaseContext<Artist> ctx, Artist artist) {
			
			var key = string.Format("ArtistQueries.SharedArtistStatsContract.{0}", artist.Id);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {

				try {

					var stats = ctx.Query()
						.Where(a => a.Id == artist.Id)
						.Select(a => new {
							FollowCount = a.Users.Count,
							AlbumCount = a.AllAlbums.Count(l => !l.Album.Deleted),
							RatedAlbumCount = a.AllAlbums.Count(l => !l.Album.Deleted && l.Album.RatingCount > 0),
							SongCount = a.AllSongs.Count(s => !s.Song.Deleted),
							RatedSongCount = a.AllSongs.Count(s => !s.Song.Deleted && s.Song.RatingScore > 0),
							AlbumRatingsTotalCount = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingCount) : 0,
							AlbumRatingsTotalSum = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingTotal) : 0
						})
						.FirstOrDefault();

					return new SharedArtistStatsContract {
						AlbumCount = stats.AlbumCount,
						FollowerCount = stats.FollowCount,
						RatedAlbumCount = stats.RatedAlbumCount,
						SongCount = stats.SongCount,
						RatedSongCount = stats.RatedSongCount,
						AlbumRatingAverage = (stats.AlbumRatingsTotalCount > 0 ? Math.Round(stats.AlbumRatingsTotalSum / (double)stats.AlbumRatingsTotalCount, 2) : 0)
					};

				} catch (HibernateException x) {
					// TODO: Loading of stats timeouts sometimes. Since they're not essential we can accept returning only partial stats.
					// However, this should be fixed by tuning the queries further.
					log.Error(x, "Unable to load shared artist stats");
					return new SharedArtistStatsContract();
				}

			});
			
		}

		private ArtistMergeRecord GetMergeRecord(IDatabaseContext<Artist> session, int sourceId) {
			return session.OfType<ArtistMergeRecord>().Query().FirstOrDefault(s => s.Source == sourceId);
		}

		public ArtistQueries(IArtistRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IEntryThumbPersister imagePersister, IEntryPictureFilePersister pictureFilePersister,
			ObjectCache cache, IUserIconFactory userIconFactory, IEnumTranslations enumTranslations)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.pictureFilePersister = pictureFilePersister;
			this.cache = cache;
			this.userIconFactory = userIconFactory;
			this.enumTranslations = enumTranslations;

		}

		public ICommentQueries Comments(IDatabaseContext<Artist> ctx) {
			return new CommentQueries<ArtistComment, Artist>(ctx.OfType<ArtistComment>(), PermissionContext, userIconFactory, entryLinkFactory);
		}

		public ArchivedArtistVersion Archive(IDatabaseContext<Artist> ctx, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "") {

			ctx.AuditLogger.SysLog("Archiving " + artist);

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		public ArchivedArtistVersion Archive(IDatabaseContext<Artist> ctx, Artist artist, ArtistArchiveReason reason, string notes = "") {

			return Archive(ctx, artist, new ArtistDiff(), reason, notes);

		}

		public ArtistContract Create(CreateArtistContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Artist needs at least one name", "contract");

			VerifyManageDatabase();

			var diff = new ArtistDiff { Names = true };

			return repository.HandleTransaction(ctx => {

				ctx.AuditLogger.SysLog(string.Format("creating a new artist with name '{0}'", contract.Names.First().Value));

				var artist = new Artist { 
					ArtistType = contract.ArtistType, 
					Description = new EnglishTranslatedString(contract.Description.Trim())
				};

				artist.Names.Init(contract.Names, artist);

				if (contract.WebLink != null) {
					artist.CreateWebLink(contract.WebLink.Description, contract.WebLink.Url, contract.WebLink.Category);
					diff.WebLinks = true;
				}

				artist.Status = (contract.Draft || !(new ArtistValidator().IsValid(artist))) ? EntryStatus.Draft : EntryStatus.Finished;

				ctx.Save(artist);

				if (contract.PictureData != null) {

					var pictureData = contract.PictureData;
					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Picture = true;

				}

				var archived = Archive(ctx, artist, diff, ArtistArchiveReason.Created);
				ctx.Update(artist);

				ctx.AuditLogger.AuditLog(string.Format("created artist {0} ({1})", entryLinkFactory.CreateEntryLink(artist), artist.ArtistType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Created, archived);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public CommentForApiContract CreateComment(int artistId, CommentForApiContract contract) {

			return HandleTransaction(ctx => Comments(ctx).Create(artistId, contract));

		}

		public bool CreateReport(int artistId, ArtistReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory, report => report.Entry.Id == artistId, 
					(artist, reporter, notesTruncated) => new ArtistReport(artist, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != ArtistReportType.Other ? enumTranslations.ArtistReportTypeNames[reportType] : null,
					artistId, reportType, hostname, notes);
			});

		}

		public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName, string url) {

			var names = anyName.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToArray();
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
					.Distinct() : new Artist[] { });

				var linkMatches = !string.IsNullOrEmpty(urlTrimmed) ?
					session.Query<ArtistWebLink>()
					.Where(w => w.Url == urlTrimmed && !w.Entry.Deleted)
					.Select(w => w.Entry)
					.Take(10)
					.ToArray()
					.Distinct() : new Artist[] { };

				return nameMatches.Union(linkMatches)
					.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public T Get<T>(int id, Func<Artist, T> fac) {

			return HandleQuery(ctx => fac(ctx.Load(id)));

		}

		public CommentForApiContract[] GetComments(int artistId) {
			
			return HandleQuery(ctx => ctx.Load(artistId).Comments.Select(c => new CommentForApiContract(c, userIconFactory, true)).ToArray());

		}

		public ArtistDetailsContract GetDetails(int id) {

			return HandleQuery(session => {

				var artist = session.Load(id);

				var stats = session.Query()
					.Where(a => a.Id == id)
					.Select(a => new {
						CommentCount = a.Comments.Count,
					})
					.FirstOrDefault();

				if (stats == null)
					EntityNotFoundException.Throw<Artist>(id);

				var contract = new ArtistDetailsContract(artist, LanguagePreference) {
					CommentCount = stats.CommentCount,
					SharedStats = GetSharedArtistStats(session, artist),
					PersonalStats = GetPersonalArtistStats(session, artist),
					AdvancedStats = GetAdvancedStats(session, artist)
				};

				if (PermissionContext.IsLoggedIn) {

					var subscription = session.OfType<ArtistForUser>()
						.Query()
						.FirstOrDefault(s => s.Artist.Id == id && s.User.Id == PermissionContext.LoggedUserId);

					if (subscription != null) {
						contract.IsAdded = true;
						contract.EmailNotifications = subscription.EmailNotifications;
						contract.SiteNotifications = subscription.SiteNotifications;
					}

				}

				var relations = (new ArtistRelationsQuery(session, LanguagePreference)).GetRelations(artist, ArtistRelationsFields.All);
				contract.LatestAlbums = relations.LatestAlbums;
				contract.TopAlbums = relations.PopularAlbums;
				contract.LatestSongs = relations.LatestSongs;
				contract.TopSongs = relations.PopularSongs;

				// If song and album counts are out of date and we know there's more albums/songs than that, update counts.
				contract.SharedStats.AlbumCount = Math.Max(contract.SharedStats.AlbumCount, contract.LatestAlbums.Length + contract.TopAlbums.Length);
				contract.SharedStats.SongCount = Math.Max(contract.SharedStats.SongCount, contract.LatestSongs.Length + contract.TopSongs.Length);

				contract.LatestComments = Comments(session).GetList(id, 3);

				if (artist.Deleted) {
					var mergeEntry = GetMergeRecord(session, id);
					contract.MergedTo = (mergeEntry != null ? new ArtistContract(mergeEntry.Target, LanguagePreference) : null);
				}

				return contract;

			});

		}

		public T GetWithMergeRecord<T>(int id, Func<Artist, ArtistMergeRecord, IDatabaseContext<Artist>, T> fac) {

			return HandleQuery(session => {
				var artist = session.Load(id);
				return fac(artist, (artist.Deleted ? GetMergeRecord(session, id) : null), session);
			});

		}

		public EntryForPictureDisplayContract GetPictureThumb(int artistId) {
			
			var size = new Size(ImageHelper.DefaultThumbSize, ImageHelper.DefaultThumbSize);

			return repository.HandleQuery(ctx => {
				
				var artist = ctx.Load(artistId);

				if (artist.Picture == null || string.IsNullOrEmpty(artist.PictureMime) || artist.Picture.HasThumb(size))
					return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference, size);

				var data = new EntryThumb(artist, artist.PictureMime);

				if (imagePersister.HasImage(data, ImageSize.Thumb)) {
					using (var stream = imagePersister.GetReadStream(data, ImageSize.Thumb)) {
						var bytes = StreamHelper.ReadStream(stream);
						return EntryForPictureDisplayContract.Create(artist, data.Mime, bytes, PermissionContext.LanguagePreference);
					}
				}

				return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference, size);

			});

		}

		public TagUsageForApiContract[] GetTopTagsForSongsAndAlbums(int artistId) {

			return repository.HandleQuery(ctx => {

				var artistTags = ctx.Load<Artist>(artistId).Tags.Tags.Select(t => t.Id);

				var albumUsages = ctx.Query<AlbumTagUsage>()
					.Where(u => !artistTags.Contains(u.Tag.Id) 
						&& u.Tag.CategoryName != Tag.CommonCategory_Lyrics 
						&& u.Album.AllArtists.Any(a => !a.IsSupport && a.Artist.Id == artistId))
					.GroupBy(t => t.Tag.Id)
					.Select(t => new { TagId = t.Key, Count = t.Count() })
					.OrderByDescending(t => t.Count)
					.Take(3)
					.ToArray();

				var songUsages = ctx.Query<SongTagUsage>()
					.Where(u => !artistTags.Contains(u.Tag.Id) 
						&& u.Tag.CategoryName != Tag.CommonCategory_Lyrics 
						&& u.Song.AllArtists.Any(a => !a.IsSupport && a.Artist.Id == artistId))
					.GroupBy(t => t.Tag.Id)
					.Select(t => new { TagId = t.Key, Count = t.Count() })
					.OrderByDescending(t => t.Count)
					.Take(3)
					.ToArray();

				var topUsages = albumUsages.Concat(songUsages)
					.GroupBy(t => t.TagId)
					.Select(t => new { TagId = t.Key, Count = t.Sum(t2 => t2.Count) })
					.OrderByDescending(t => t.Count)
					.Take(3)
					.ToArray();

				var tags = ctx.LoadMultiple<Tag>(topUsages.Select(t => t.TagId)).ToDictionary(t => t.Id);

				return topUsages.Select(t => new TagUsageForApiContract(tags[t.TagId], t.Count, LanguagePreference)).ToArray();

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return new TagUsageQueries(PermissionContext).RemoveTagUsage<ArtistTagUsage, Artist>(tagUsageId, repository);

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

				session.AuditLogger.SysLog("reverting " + artist + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedArtistContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();
				var diff = new ArtistDiff();

				artist.ArtistType = fullProperties.ArtistType;
				artist.Description.Original = fullProperties.Description;
				artist.Description.English = fullProperties.DescriptionEng ?? string.Empty;
				artist.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;
				artist.BaseVoicebank = DatabaseContextHelper.RestoreWeakRootEntityRef(session, warnings, fullProperties.BaseVoicebank);

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);

				if (versionWithPic != null) {

					artist.Picture = versionWithPic.Picture;
					artist.PictureMime = versionWithPic.PictureMime;

					if (versionWithPic.Picture != null) {

						var thumbGenerator = new ImageThumbGenerator(imagePersister);
						using (var stream = new MemoryStream(versionWithPic.Picture.Bytes)) {
							var thumb = new EntryThumb(artist, versionWithPic.PictureMime);
							thumbGenerator.GenerateThumbsAndMoveImage(stream, thumb, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);
						}

					}


				} else {

					artist.Picture = null;
					artist.PictureMime = null;

				}

				// Assume picture was changed if there's a version between the current version and the restored version where the picture was changed.
				diff.Picture = !Equals(artist.ArchivedVersionsManager.GetLatestVersionWithField(ArtistEditableFields.Picture, artist.Version), versionWithPic);

				// Groups
				DatabaseContextHelper.RestoreObjectRefs<GroupForArtist, Artist>(
					session, warnings, artist.AllGroups, fullProperties.Groups, (a1, a2) => (a1.Group.Id == a2.Id),
					grp => (!artist.HasGroup(grp) ? artist.AddGroup(grp) : null),
					groupForArtist => groupForArtist.Delete());

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = artist.Names.SyncByContent(fullProperties.Names, artist);
					session.Sync(nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(artist.WebLinks, fullProperties.WebLinks, artist);
					session.Sync(webLinkDiff);
				}

				Archive(session, artist, diff, ArtistArchiveReason.Reverted, string.Format("Reverted to version {0}", archivedVersion.Version));
				AuditLog(string.Format("reverted {0} to revision {1}", entryLinkFactory.CreateEntryLink(artist), archivedVersion.Version), session);

				return new EntryRevertedContract(artist, warnings);

			});

		}

		public int Update(ArtistForEditContract properties, EntryPictureFileContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			return repository.HandleTransaction(ctx => {

				var artist = ctx.Load(properties.Id);

				VerifyEntryEdit(artist);

				var diff = new ArtistDiff(DoSnapshot(artist.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(permissionContext)));

				ctx.AuditLogger.SysLog(string.Format("updating properties for {0}", artist));

				if (artist.ArtistType != properties.ArtistType) {
					artist.ArtistType = properties.ArtistType;
					diff.ArtistType = true;
				}

				diff.Description = artist.Description.CopyFrom(properties.Description);

				if (artist.TranslatedName.DefaultLanguage != properties.DefaultNameLanguage) {
					artist.TranslatedName.DefaultLanguage = properties.DefaultNameLanguage;
					diff.OriginalName = true;
				}

				// Required because of a bug in NHibernate
				NHibernateUtil.Initialize(artist.Picture);

				if (pictureData != null) {

					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Picture = true;

				}

				if (artist.Status != properties.Status) {
					artist.Status = properties.Status;
					diff.Status = true;
				}

				var nameDiff = artist.Names.Sync(properties.Names, artist);
				ctx.OfType<ArtistName>().Sync(nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				if (!artist.BaseVoicebank.NullSafeIdEquals(properties.BaseVoicebank)) {
					
					var newBase = ctx.NullSafeLoad(properties.BaseVoicebank);

					if (artist.IsValidBaseVoicebank(newBase)) {
						diff.BaseVoicebank = true;
						artist.SetBaseVoicebank(ctx.NullSafeLoad(properties.BaseVoicebank));
					}

				}

				var webLinkDiff = WebLink.Sync(artist.WebLinks, properties.WebLinks, artist);
				ctx.OfType<ArtistWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				if (diff.ArtistType || diff.Names) {

					foreach (var song in artist.Songs) {
						song.Song.UpdateArtistString();
						ctx.Update(song);
					}

				}

				var groupsDiff = CollectionHelper.Diff(artist.Groups, properties.Groups, (i, i2) => (i.Id == i2.Id));

				foreach (var grp in groupsDiff.Removed) {
					grp.Delete();
					ctx.Delete(grp);
				}

				foreach (var grp in groupsDiff.Added) {
					var link = artist.AddGroup(ctx.Load(grp.Group.Id));
					ctx.Save(link);
				}

				if (groupsDiff.Changed)
					diff.Groups = true;

				var picsDiff = artist.Pictures.SyncPictures(properties.Pictures, ctx.OfType<User>().GetLoggedUser(permissionContext), artist.CreatePicture);
				ctx.OfType<ArtistPictureFile>().Sync(picsDiff);
				var entryPictureFileThumbGenerator = new ImageThumbGenerator(pictureFilePersister);
				artist.Pictures.GenerateThumbsAndMoveImage(entryPictureFileThumbGenerator, picsDiff.Added, ImageSizes.Original | ImageSizes.Thumb);

				if (picsDiff.Changed)
					diff.Pictures = true;

				var logStr = string.Format("updated properties for artist {0} ({1})", entryLinkFactory.CreateEntryLink(artist), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				var archived = Archive(ctx, artist, diff, ArtistArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				ctx.Update(artist);

				ctx.AuditLogger.AuditLog(logStr);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Updated, archived);

				return artist.Id;

			});

		}

	}

}