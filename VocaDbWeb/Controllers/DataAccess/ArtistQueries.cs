using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using NHibernate;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Artist"/>.
	/// </summary>
	public class ArtistQueries : QueriesBase<IArtistRepository, Artist> {

		private readonly ObjectCache cache;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryThumbPersister imagePersister;
		private readonly IEntryPictureFilePersister pictureFilePersister;
		private readonly IUserIconFactory userIconFactory;

		class CachedAdvancedArtistStatsContract {
			
			public TopStatContract<TranslatedArtistContract>[] TopVocaloids { get; set; }

		}

		private AdvancedArtistStatsContract GetAdvancedStats(IRepositoryContext<Artist> ctx, Artist artist) {
			
			if (artist.ArtistType != ArtistType.Producer)
				return null;

			var key = string.Format("ArtistQueries.AdvancedArtistStatsContract.{0}", artist.Id);

			var cached = cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(24), () => {

				var types = new[] { ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVoiceSynthesizer };

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

		private PersonalArtistStatsContract GetPersonalArtistStats(IRepositoryContext<Artist> ctx, Artist artist) {
			
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

		private SharedArtistStatsContract GetSharedArtistStats(IRepositoryContext<Artist> ctx, Artist artist) {
			
			var key = string.Format("ArtistQueries.SharedArtistStatsContract.{0}", artist.Id);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () => {
				
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

			});
			
		}

		private ArtistMergeRecord GetMergeRecord(IRepositoryContext<Artist> session, int sourceId) {
			return session.OfType<ArtistMergeRecord>().Query().FirstOrDefault(s => s.Source == sourceId);
		}

		public ArtistQueries(IArtistRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, 
			IEntryThumbPersister imagePersister, IEntryPictureFilePersister pictureFilePersister,
			ObjectCache cache, IUserIconFactory userIconFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.pictureFilePersister = pictureFilePersister;
			this.cache = cache;
			this.userIconFactory = userIconFactory;

		}

		public ICommentQueries Comments(IRepositoryContext<Artist> ctx) {
			return new CommentQueries<ArtistComment, Artist>(ctx.OfType<ArtistComment>(), PermissionContext, userIconFactory, entryLinkFactory);
		}

		public ArchivedArtistVersion Archive(IRepositoryContext<Artist> ctx, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "") {

			ctx.AuditLogger.SysLog("Archiving " + artist);

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		public ArchivedArtistVersion Archive(IRepositoryContext<Artist> ctx, Artist artist, ArtistArchiveReason reason, string notes = "") {

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

			ParamIs.NotNull(() => contract);

			return HandleTransaction(ctx => Comments(ctx).Create(artistId, contract));

		}

		public bool CreateReport(int artistId, ArtistReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory, report => report.Artist.Id == artistId, 
					(artist, reporter, notesTruncated) => new ArtistReport(artist, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != ArtistReportType.Other ? Translate.ArtistReportTypeNames[reportType] : null,
					artistId, reportType, hostname, notes);
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

		public T GetWithMergeRecord<T>(int id, Func<Artist, ArtistMergeRecord, IRepositoryContext<Artist>, T> fac) {

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

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(artist.WebLinks, validWebLinks, artist);
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