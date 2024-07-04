#nullable disable

using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;
using System.Web;
using NHibernate;
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
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Comments;
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
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Database.Queries;

/// <summary>
/// Database queries related to <see cref="Album"/>.
/// </summary>
public class AlbumQueries : QueriesBase<IAlbumRepository, Album>
{
	private readonly ObjectCache _cache;
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly IAggregatedEntryImageUrlFactory _imageUrlFactory;
	private readonly IEnumTranslations _enumTranslations;
	private readonly IFollowedArtistNotifier _followedArtistNotifier;
	private readonly IEntryThumbPersister _imagePersister;
	private readonly IEntryPictureFilePersister _pictureFilePersister;
	private readonly IUserMessageMailer _mailer;
	private readonly IPVParser _pvParser;
	private readonly IUserIconFactory _userIconFactory;
	private readonly IDiscordWebhookNotifier _discordWebhookNotifier;

	private IEntryLinkFactory EntryLinkFactory => _entryLinkFactory;

	private async Task<ArchivedSongVersion> ArchiveSongAsync(IDatabaseContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "")
	{
		var agentLoginData = await ctx.CreateAgentLoginDataAsync(PermissionContext);
		var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
		await ctx.OfType<ArchivedSongVersion>().SaveAsync(archived);
		return archived;
	}

	private async Task<List<Artist>> GetArtistsAsync(IDatabaseContext<Album> ctx, ArtistContract[] artistContracts)
	{
		var ids = artistContracts.Select(a => a.Id).ToArray();
		return await ctx.Query<Artist>().WhereIdIn(ids).VdbToListAsync();
	}

	private AlbumMergeRecord GetMergeRecord(IDatabaseContext session, int sourceId)
	{
		return session.Query<AlbumMergeRecord>().FirstOrDefault(s => s.Source == sourceId);
	}

#nullable enable
	/// <summary>
	/// Stats shared for all users. These are cached for 1 hour.
	/// </summary>
	private SharedAlbumStatsContract GetSharedAlbumStats(IDatabaseContext ctx, Album album)
	{
		var key = $"AlbumQueries.SharedAlbumStatsContract.{album.Id}";
		return _cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () =>
		{
			var latestReview = album.LastReview;
			var latestRatingScore = latestReview != null ? album.UserCollections.FirstOrDefault(uc => uc.User.Equals(latestReview.Author)) : null;

			return new SharedAlbumStatsContract
			{
				ReviewCount = album.Reviews.Count(),
				LatestReview = latestReview != null ? new AlbumReviewContract(latestReview, _userIconFactory) : null,
				LatestReviewRatingScore = latestRatingScore?.Rating ?? 0,
				OwnedCount = album.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Owned),
				WishlistCount = album.UserCollections.Count(au => au.PurchaseStatus == PurchaseStatus.Wishlisted),
			};
		});
	}
#nullable disable

	private ArtistForAlbum RestoreArtistRef(Album album, Artist artist, ArchivedArtistForAlbumContract albumRef)
	{
		if (artist != null)
		{
			return (!artist.HasAlbum(album) ? artist.AddAlbum(album, albumRef.IsSupport, albumRef.Roles) : null);
		}
		else
		{
			return album.AddArtist(albumRef.NameHint, albumRef.IsSupport, albumRef.Roles);
		}
	}

	private SongInAlbum RestoreTrackRef(Album album, Song song, SongInAlbumRefContract songRef)
	{
		if (song != null)
		{
			return (!album.HasSong(song) ? album.AddSong(song, songRef.TrackNumber, songRef.DiscNumber) : null);
		}
		else
		{
			return album.AddSong(songRef.NameHint, songRef.TrackNumber, songRef.DiscNumber);
		}
	}

	private async Task UpdateSongArtistsAsync(IDatabaseContext<Album> ctx, Song song, ArtistContract[] artistContracts)
	{
		var artistDiff = await song.SyncArtistsAsync(artistContracts,
			addedArtistContracts => GetArtistsAsync(ctx, addedArtistContracts));

		await ctx.SyncAsync(artistDiff);

		if (artistDiff.Changed)
		{
			var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(PermissionContext)));
			diff.Artists.Set();

			song.UpdateArtistString();
			var archived = await ArchiveSongAsync(ctx.OfType<Song>(), song, diff, SongArchiveReason.PropertiesUpdated);
			await ctx.UpdateAsync(song);

			await ctx.AuditLogger.AuditLogAsync("updated artists for " + _entryLinkFactory.CreateEntryLink(song));
			await AddEntryEditedEntryAsync(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Updated, archived);
		}
	}

	public AlbumQueries(
		IAlbumRepository repository,
		IUserPermissionContext permissionContext,
		IEntryLinkFactory entryLinkFactory,
		IEntryThumbPersister imagePersister,
		IEntryPictureFilePersister pictureFilePersister,
		IUserMessageMailer mailer,
		IUserIconFactory userIconFactory,
		IEnumTranslations enumTranslations,
		IPVParser pvParser,
		IFollowedArtistNotifier followedArtistNotifier,
		IAggregatedEntryImageUrlFactory entryThumbPersister,
		ObjectCache cache,
		IDiscordWebhookNotifier discordWebhookNotifier
	)
		: base(repository, permissionContext)
	{
		_entryLinkFactory = entryLinkFactory;
		_imagePersister = imagePersister;
		_pictureFilePersister = pictureFilePersister;
		_mailer = mailer;
		_userIconFactory = userIconFactory;
		_enumTranslations = enumTranslations;
		_pvParser = pvParser;
		_followedArtistNotifier = followedArtistNotifier;
		_imageUrlFactory = entryThumbPersister;
		_cache = cache;
		_discordWebhookNotifier = discordWebhookNotifier;
	}

	public AlbumReviewContract AddReview(int albumId, AlbumReviewContract contract)
	{
		PermissionContext.VerifyPermission(PermissionToken.CreateComments);

		return HandleTransaction(ctx =>
		{
			AlbumReview review = null;

			if (contract.Id != 0)
			{
				review = ctx.Load<AlbumReview>(contract.Id);
				if (!review.Author.Equals(PermissionContext.LoggedUser))
				{
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);
				}
			}
			else
			{
				review = ctx.Query<AlbumReview>().FirstOrDefault(r => r.EntryForComment.Id == albumId && r.Author.Id == PermissionContext.LoggedUserId && r.LanguageCode == contract.LanguageCode);
			}

			// Create
			if (review == null || review.Deleted)
			{
				var album = ctx.Load<Album>(albumId);
				var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
				review = new AlbumReview(album, contract.Text, agentLoginData, contract.Title, contract.LanguageCode);
				album.AllReviews.Add(review);
				ctx.Save(review);
			}
			else
			{ // Update
				review.LanguageCode = contract.LanguageCode;
				review.Message = contract.Text;
				review.Title = contract.Title;
				ctx.Update(review);
			}

			ctx.AuditLogger.AuditLog($"submitted review for {_entryLinkFactory.CreateEntryLink(review.EntryForComment)}");

			return new AlbumReviewContract(review, _userIconFactory);
		});
	}

	public void DeleteReview(int reviewId)
	{
		PermissionContext.VerifyPermission(PermissionToken.EditProfile);

		_repository.HandleTransaction(ctx =>
		{
			var review = ctx.Load<AlbumReview>(reviewId);

			if (!review.Author.Equals(PermissionContext.LoggedUser))
			{
				PermissionContext.VerifyPermission(PermissionToken.DeleteComments);
			}

			review.Delete();
			ctx.Update(review);
		});
	}

	public async Task<IEnumerable<AlbumReviewContract>> GetReviews(int albumId, string languageCode)
	{
		return await _repository.HandleQueryAsync(async ctx =>
		{
			var album = await ctx.LoadAsync(albumId);

			return album.Reviews
				.Where(review => string.IsNullOrEmpty(languageCode) || review.LanguageCode == languageCode)
				.OrderBy(review => review.Created)
				.Select(review => new AlbumReviewContract(review, _userIconFactory))
				.ToArray();
		});
	}

	public async Task<IEnumerable<AlbumForUserForApiContract>> GetUserCollections(int albumId, ContentLanguagePreference languagePreference)
	{
		return await _repository.HandleQueryAsync(async ctx =>
		{
			var album = await ctx.LoadAsync(albumId);

			return album.UserCollections
				.Select(uc => new AlbumForUserForApiContract(
					uc,
					languagePreference,
					PermissionContext,
					_imageUrlFactory,
					AlbumOptionalFields.None,
					shouldShowCollectionStatus: uc.User.Id == PermissionContext.LoggedUserId || uc.User.Options.PublicAlbumCollection,
					includeUser: uc.User.Id == PermissionContext.LoggedUserId || uc.User.Options.PublicAlbumCollection
				))
				.ToArray();
		});
	}

	public ArchivedAlbumVersion Archive(IDatabaseContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "")
	{
		var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
		var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
		ctx.OfType<ArchivedAlbumVersion>().Save(archived);
		return archived;
	}

	public async Task<ArchivedAlbumVersion> ArchiveAsync(IDatabaseContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "")
	{
		var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
		var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
		await ctx.SaveAsync(archived);
		return archived;
	}

	public ArchivedAlbumVersion Archive(IDatabaseContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "")
	{
		return Archive(ctx, album, new AlbumDiff(), reason, notes);
	}

	public async Task<ArchivedAlbumVersion> ArchiveAsync(IDatabaseContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "")
	{
		return await ArchiveAsync(ctx, album, new AlbumDiff(), reason, notes);
	}

	public ICommentQueries Comments(IDatabaseContext<Album> ctx)
	{
		return new CommentQueries<AlbumComment, Album>(ctx.OfType<AlbumComment>(), PermissionContext, _userIconFactory, _entryLinkFactory);
	}

#nullable enable
	public async Task<AlbumContract> Create(CreateAlbumForApiContract contract)
	{
		ParamIs.NotNull(() => contract);

		if (contract.Names == null || !contract.Names.Any())
			throw new ArgumentException("Album needs at least one name", "contract");

		VerifyManageDatabase();

		return await _repository.HandleTransactionAsync(async ctx =>
		{
			ctx.AuditLogger.SysLog($"creating a new album with name '{contract.Names.First().Value}'");

			var album = new Album { DiscType = contract.DiscType };

			album.Names.Init(contract.Names, album);

			await ctx.SaveAsync(album);

			foreach (var artistContract in contract.Artists)
			{
				var artist = await ctx.LoadAsync<Artist>(artistContract.Id);
				if (!album.HasArtist(artist))
				{
					await ctx.SaveAsync(artist.AddAlbum(album));
				}
			}

			album.UpdateArtistString();
			var archived = await ArchiveAsync(ctx, album, AlbumArchiveReason.Created);
			await ctx.UpdateAsync(album);

			await ctx.AuditLogger.AuditLogAsync($"created album {_entryLinkFactory.CreateEntryLink(album)} ({album.DiscType})");
			await AddEntryEditedEntryAsync(ctx.OfType<ActivityEntry>(), album, EntryEditEvent.Created, archived);

			await _followedArtistNotifier.SendNotificationsAsync(ctx, album, album.ArtistList, PermissionContext.LoggedUser);

			return new AlbumContract(album, PermissionContext.LanguagePreference, PermissionContext);
		});
	}

	public CommentForApiContract CreateComment(int albumId, CommentForApiContract contract)
	{
		return HandleTransaction(ctx => Comments(ctx).Create(albumId, contract));
	}

	public Task<(bool created, int reportId)> CreateReport(int albumId, AlbumReportType reportType, string hostname, string notes, int? versionNumber)
	{
		ParamIs.NotNull(() => hostname);
		ParamIs.NotNull(() => notes);

		return HandleTransactionAsync(ctx =>
		{
			return new Service.Queries.EntryReportQueries().CreateReport(
				ctx,
				PermissionContext,
				_entryLinkFactory,
				(album, reporter, notesTruncated) => new AlbumReport(album, reportType, reporter, hostname, notesTruncated, versionNumber),
				() => reportType != AlbumReportType.Other ? _enumTranslations.AlbumReportTypeNames[reportType] : null,
				albumId,
				reportType,
				hostname,
				notes,
				_discordWebhookNotifier,
				AlbumReport.ReportTypesWithRequiredNotes
			);
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
	public AlbumDetailsContract GetAlbumDetails(int id, string hostname)
	{
		return HandleQuery(session =>
		{
			var album = session.Load<Album>(id);

			var user = PermissionContext.LoggedUser;

			SongVoteRating? GetRatingFunc(Song song)
			{
				return user != null && song != null ? (SongVoteRating?)session.Query<FavoriteSongForUser>().Where(s => s.Song.Id == song.Id && s.User.Id == user.Id).Select(r => r.Rating).FirstOrDefault() : null;
			}

			var contract = new AlbumDetailsContract(album, PermissionContext.LanguagePreference, PermissionContext, _imageUrlFactory, GetRatingFunc,
				discTypeTag: new EntryTypeTags(session).GetTag(EntryType.Album, album.DiscType))
			{
				CommentCount = Comments(session).GetCount(id),
				Hits = session.Query<AlbumHit>().Count(h => h.Entry.Id == id),
				Stats = GetSharedAlbumStats(session, album)
			};

			if (user != null)
			{
				var albumForUser = session.Query<AlbumForUser>()
					.FirstOrDefault(a => a.Album.Id == id && a.User.Id == user.Id);

				contract.AlbumForUser = albumForUser != null
					? new AlbumForUserContract(
						albumForUser,
						PermissionContext.LanguagePreference,
						PermissionContext,
						_userIconFactory
					)
					: null;
			}

			contract.LatestComments = session.Query<AlbumComment>()
				.WhereNotDeleted()
				.Where(c => c.EntryForComment.Id == id)
				.OrderByDescending(c => c.Created)
				.Take(3)
				.ToArray()
				.Select(c => new CommentForApiContract(c, _userIconFactory))
				.ToArray();

			if (album.Deleted)
			{
				var mergeEntry = GetMergeRecord(session, id);
				contract.MergedTo = (mergeEntry != null ? new AlbumContract(mergeEntry.Target, LanguagePreference, PermissionContext) : null);
			}

			if (user != null || !string.IsNullOrEmpty(hostname))
			{
				var agentNum = (user != null ? user.Id : hostname.GetHashCode());

				using var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted);
				var isHit = session.Query<AlbumHit>().Any(h => h.Entry.Id == id && h.Agent == agentNum);

				if (!isHit)
				{
					var hit = new AlbumHit(album, agentNum);
					session.Save(hit);

					try
					{
						tx.Commit();
					}
					catch (SqlException x)
					{
						session.AuditLogger.SysLog("Error while committing hit: " + x.Message);
					}
				}
			}

			return contract;
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
	public AlbumDetailsForApiContract GetAlbumDetailsForApi(int id, string hostname)
	{
		return HandleQuery(session =>
		{
			var album = session.Load<Album>(id);

			var user = PermissionContext.LoggedUser;

			SongVoteRating? GetRatingFunc(Song song)
			{
				return user is not null && song is not null
					? session.Query<FavoriteSongForUser>()
						.Where(s => s.Song.Id == song.Id && s.User.Id == user.Id)
						.Select(r => r.Rating)
						.FirstOrDefault()
					: null;
			}

			var contract = new AlbumDetailsForApiContract(
				album: album,
				languagePreference: PermissionContext.LanguagePreference,
				userContext: PermissionContext,
				thumbPersister: _imageUrlFactory,
				getSongRating: GetRatingFunc,
				discTypeTag: new EntryTypeTags(session).GetTag(entryType: EntryType.Album, subType: album.DiscType)
			)
			{
				CommentCount = Comments(session).GetCount(id),
				Hits = session.Query<AlbumHit>().Count(h => h.Entry.Id == id),
				Stats = GetSharedAlbumStats(session, album),
			};

			if (user is not null)
			{
				var albumForUser = session.Query<AlbumForUser>().FirstOrDefault(a => a.Album.Id == id && a.User.Id == user.Id);

				contract.AlbumForUser = albumForUser is not null
					? new AlbumForUserForApiContract(
						albumForUser: albumForUser,
						languagePreference: PermissionContext.LanguagePreference,
						PermissionContext,
						thumbPersister: null,
						fields: AlbumOptionalFields.None,
						shouldShowCollectionStatus: true
					)
					: null;
			}

			contract.LatestComments = session.Query<AlbumComment>()
				.WhereNotDeleted()
				.Where(c => c.EntryForComment.Id == id)
				.OrderByDescending(c => c.Created)
				.Take(3)
				.ToArray()
				.Select(c => new CommentForApiContract(comment: c, iconFactory: _userIconFactory))
				.ToArray();

			if (album.Deleted)
			{
				var mergeEntry = GetMergeRecord(session, id);

				contract.MergedTo = mergeEntry is not null
					? new AlbumForApiContract(
						album: mergeEntry.Target,
						languagePreference: LanguagePreference,
						PermissionContext,
						thumbPersister: null,
						fields: AlbumOptionalFields.None
					)
					: null;
			}

			if (user is not null || !string.IsNullOrEmpty(hostname))
			{
				var agentNum = user is not null ? user.Id : hostname.GetHashCode();

				using var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted);
				var isHit = session.Query<AlbumHit>().Any(h => h.Entry.Id == id && h.Agent == agentNum);

				if (!isHit)
				{
					var hit = new AlbumHit(album, agentNum);
					session.Save(hit);

					try
					{
						tx.Commit();
					}
					catch (SqlException x)
					{
						session.AuditLogger.SysLog("Error while committing hit: " + x.Message);
					}
				}
			}

			return contract;
		});
	}
#nullable disable

	public T GetAlbumWithMergeRecord<T>(int id, Func<Album, AlbumMergeRecord, T> fac)
	{
		return HandleQuery(session =>
		{
			var album = session.Load<Album>(id);
			return fac(album, (album.Deleted ? GetMergeRecord(session, id) : null));
		});
	}

	public CommentForApiContract[] GetComments(int albumId)
	{
		return HandleQuery(ctx => Comments(ctx).GetAll(albumId));
	}

	public EntryForPictureDisplayContract GetCoverPictureThumb(int albumId)
	{
		var size = ImageSize.Thumb;

		// TODO: this all should be moved to DynamicImageUrlFactory
		return _repository.HandleQuery(ctx =>
		{
			var album = ctx.Load(albumId);

			// If there is no picture, return empty.
			if (album.CoverPictureData == null || string.IsNullOrEmpty(album.CoverPictureMime))
				return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference);

			// Try to read thumbnail from file system.
			var data = album.Thumb;
			if (_imagePersister.HasImage(data, size))
			{
				var bytes = _imagePersister.ReadBytes(data, size);
				return EntryForPictureDisplayContract.Create(album, data.Mime, bytes, PermissionContext.LanguagePreference);
			}

			// This should return the original image.
			return EntryForPictureDisplayContract.Create(album, PermissionContext.LanguagePreference);
		});
	}

	public AlbumForEditForApiContract GetForEdit(int id)
	{
		return HandleQuery(session =>
			new AlbumForEditForApiContract(
				album: session.Load<Album>(id),
				languagePreference: PermissionContext.LanguagePreference,
				imageStore: _imageUrlFactory,
				permissionContext: PermissionContext
			)
		);
	}

	public RelatedAlbumsContract GetRelatedAlbums(int albumId, AlbumOptionalFields fields, ContentLanguagePreference? lang = null)
	{

		var language = lang ?? _permissionContext.LanguagePreference;

		return _repository.HandleQuery(ctx =>
		{
			var album = ctx.Load(albumId);
			var q = new RelatedAlbumsQuery(ctx);
			var albums = q.GetRelatedAlbums(album);

			return new RelatedAlbumsContract
			{
				ArtistMatches =
					albums.ArtistMatches
					.Select(a => new AlbumForApiContract(a, null, language, PermissionContext, null, fields, SongOptionalFields.None))
					.OrderBy(a => a.Name)
					.ToArray(),
				LikeMatches =
					albums.LikeMatches
					.Select(a => new AlbumForApiContract(a, null, language, PermissionContext, null, fields, SongOptionalFields.None))
					.OrderBy(a => a.Name)
					.ToArray(),
				TagMatches =
					albums.TagMatches
					.Select(a => new AlbumForApiContract(a, null, language, PermissionContext, null, fields, SongOptionalFields.None))
					.OrderBy(a => a.Name)
					.ToArray()
			};
		});
	}

	public Task<TagUsageForApiContract[]> GetTagSuggestions(int albumId)
	{
		var maxResults = 3;

		return _repository.HandleQueryAsync(async ctx =>
		{
			var album = ctx.Load<Album>(albumId);
			var albumTags = album.Tags.Tags.Select(t => t.Id);

			var songUsages = ctx.Query<SongTagUsage>()
				.Where(u => !albumTags.Contains(u.Tag.Id)
					&& !u.Tag.Deleted
					&& !u.Tag.HideFromSuggestions
					&& u.Entry.AllAlbums.Any(a => a.Album.Id == albumId))
				.WhereTagHasTarget(album.DiscType)
				.GroupBy(t => t.Tag.Id)
				.Select(t => new { TagId = t.Key, Count = t.Count() })
				.Where(t => t.Count > 1)
				.OrderByDescending(t => t.Count)
				.Take(maxResults)
				.ToArray();

			var tags = ctx.LoadMultiple<Tag>(songUsages.Select(t => t.TagId)).ToDictionary(t => t.Id);

			var results = songUsages.Select(t => new TagUsageForApiContract(tags[t.TagId], t.Count, LanguagePreference));

			if (songUsages.Length < 3)
			{
				var pvResults = await _pvParser.ParseByUrlsAsync(album.PVs
					.Where(pv => pv.Service == PVService.NicoNicoDouga)
					.Select(pv => pv.Url), true, _permissionContext);

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

	public IEnumerable<Dictionary<string, string>> GetTracksFormatted(int id, int? discNumber, string[] fields, ContentLanguagePreference lang)
	{
		if (fields == null || fields.Length == 0)
			fields = new[] { "id", "title" };

		return HandleQuery(db => new AlbumSongFormatter(_entryLinkFactory).ApplyFormatDict(db.Load(id), fields, discNumber, lang));
	}

	public void Merge(int sourceId, int targetId)
	{
		PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

		if (sourceId == targetId)
			throw new ArgumentException("Source and target albums can't be the same", "targetId");

		_repository.HandleTransaction(session =>
		{
			var source = session.Load(sourceId);
			var target = session.Load(targetId);

			session.AuditLogger.AuditLog($"Merging {EntryLinkFactory.CreateEntryLink(source)} to {EntryLinkFactory.CreateEntryLink(target)}");

			NHibernateUtil.Initialize(source.CoverPictureData);
			NHibernateUtil.Initialize(target.CoverPictureData);

			foreach (var n in source.Names.Names.Where(n => !target.HasName(n)))
			{
				var name = target.CreateName(n.Value, n.Language);
				session.Save(name);
			}

			foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url)))
			{
				var link = target.CreateWebLink(w.Description, w.Url, w.Category, w.Disabled);
				session.Save(link);
			}

			var artists = source.Artists.Where(a => !target.HasArtistForAlbum(a)).ToArray();
			foreach (var a in artists)
			{
				a.Move(target);
				session.Update(a);
			}

			var songs = source.Songs.Where(s => s.Song == null || !target.HasSong(s.Song)).ToArray();
			foreach (var s in songs)
			{
				s.Move(target);
				session.Update(s);
			}

			var pictures = source.Pictures.ToArray();
			foreach (var p in pictures)
			{
				p.Move(target);
				session.Update(p);
			}

			var userCollections = source.UserCollections.Where(a => !target.IsInUserCollection(a.User)).ToArray();
			foreach (var u in userCollections)
			{
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

			target.OriginalRelease.ReleaseEvents = target.OriginalRelease.ReleaseEvents.Concat(source.OriginalRelease.ReleaseEvents).Distinct().ToArray();
			
			// Tags
			source.Tags.MoveVotes(target.Tags, (tag) => new AlbumTagUsage(target, tag));

			// Create merge record
			var mergeEntry = new AlbumMergeRecord(source, target);
			session.Save(mergeEntry);

			source.Deleted = true;

			target.UpdateArtistString();
			target.Names.UpdateSortNames();

			Archive(session, source, AlbumArchiveReason.Deleted, $"Merged to {target}");
			Archive(session, target, AlbumArchiveReason.Merged, $"Merged from {source}");

			session.Update(source);
			session.Update(target);
		});
	}

	public int MoveToTrash(int albumId)
	{
		PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

		return HandleTransaction(ctx =>
		{
			var album = ctx.Load<Album>(albumId);

			AuditLog($"moving {album} to trash", ctx);

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

	public EntryRevertedContract RevertToVersion(int archivedAlbumVersionId)
	{
		PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

		return HandleTransaction(session =>
		{
			var archivedVersion = session.Load<ArchivedAlbumVersion>(archivedAlbumVersionId);

			if (archivedVersion.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			var album = archivedVersion.Album;

			session.AuditLogger.SysLog("reverting " + album + " to version " + archivedVersion.Version);

			var fullProperties = ArchivedAlbumContract.GetAllProperties(archivedVersion);
			var warnings = new List<string>();
			var diff = new AlbumDiff();

			album.Description.Original = fullProperties.Description;
			album.Description.English = fullProperties.DescriptionEng ?? string.Empty;
			album.DiscType = fullProperties.DiscType;
			album.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);

				if (versionWithPic != null)
				{
					album.CoverPictureData = versionWithPic.CoverPicture;
					album.CoverPictureMime = versionWithPic.CoverPictureMime;

					if (versionWithPic.CoverPicture != null)
					{
						var thumbGenerator = new ImageThumbGenerator(_imagePersister);
						using var stream = new MemoryStream(versionWithPic.CoverPicture.Bytes);
						var thumb = new EntryThumb(album, versionWithPic.CoverPictureMime, ImagePurpose.Main);
						thumbGenerator.GenerateThumbsAndMoveImage(stream, thumb, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);
					}
				}
				else
				{
					album.CoverPictureData = null;
					album.CoverPictureMime = null;
				}

				// Assume picture was changed if there's a version between the current version and the restored version where the picture was changed.
				diff.Cover.Set(!Equals(album.ArchivedVersionsManager.GetLatestVersionWithField(AlbumEditableFields.Cover, album.Version), versionWithPic));
			}

			// Original release
			if (fullProperties.OriginalRelease != null)
			{
				if (fullProperties.OriginalRelease.ReleaseEvents != null)
				{
					album.OriginalRelease =
						 new AlbumRelease(fullProperties.OriginalRelease, fullProperties.OriginalRelease.ReleaseEvents.Select(e => session.NullSafeLoad<ReleaseEvent>(e)).ToArray());
				}
				else if (fullProperties.OriginalRelease.ReleaseEvent != null)
				{

					album.OriginalRelease =
						 new AlbumRelease(fullProperties.OriginalRelease, new ReleaseEvent[] { session.NullSafeLoad<ReleaseEvent>(fullProperties.OriginalRelease.ReleaseEvent) });
				}

				if (fullProperties.OriginalRelease.ReleaseDate != null)
				{
					var rd = fullProperties.OriginalRelease.ReleaseDate;
					album.OriginalReleaseDate.Year = rd.Year;
					album.OriginalReleaseDate.Month = rd.Month;
					album.OriginalReleaseDate.Day = rd.Day;
				}

				if (fullProperties.OriginalRelease.CatNum != null)
				{
					album.OriginalRelease.CatNum = fullProperties.OriginalRelease.CatNum;
				}
			}

			// Artists
			DatabaseContextHelper.RestoreObjectRefs(
				session: session.OfType<Artist>(),
				warnings: warnings,
				existing: album.AllArtists,
				objRefs: fullProperties.Artists,
				equality: (a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
				createEntryFunc: (artist, albumRef) => RestoreArtistRef(album, artist, albumRef),
				deleteFunc: albumForArtist => albumForArtist.Delete()
			);

			// Songs
			DatabaseContextHelper.RestoreObjectRefs(
				session: session.OfType<Song>(),
				warnings: warnings,
				existing: album.AllSongs,
				objRefs: fullProperties.Songs,
				equality: (a1, a2) => (a1.Song != null && a1.Song.Id == a2.Id) || a1.Song == null && a2.Id == 0 && a1.Name == a2.NameHint,
				createEntryFunc: (song, songRef) => RestoreTrackRef(album, song, songRef),
				deleteFunc: songInAlbum => songInAlbum.Delete()
			);

			// Names
			if (fullProperties.Names != null)
			{
				var nameDiff = album.Names.SyncByContent(fullProperties.Names, album);
				session.Sync(nameDiff);
			}

			// Weblinks
			if (fullProperties.WebLinks != null)
			{
				var webLinkDiff = WebLink.SyncByValue(
					oldLinks: album.WebLinks,
					newLinks: fullProperties.WebLinks,
					webLinkFactory: album
				);
				session.Sync(webLinkDiff);
			}

			// PVs
			if (fullProperties.PVs != null)
			{
				var pvDiff = CollectionHelper.Diff(album.PVs, fullProperties.PVs, (p1, p2) => p1.PVId == p2.PVId && p1.Service == p2.Service);

				foreach (var pv in pvDiff.Added)
				{
					session.Save(album.CreatePV(new PVContract(pv)));
				}

				foreach (var pv in pvDiff.Removed)
				{
					pv.OnDelete();
					session.Delete(pv);
				}
			}

			album.UpdateArtistString();
			album.UpdateRatingTotals();

			Archive(session, album, diff, AlbumArchiveReason.Reverted, $"Reverted to version {archivedVersion.Version}");
			AuditLog($"reverted {EntryLinkFactory.CreateEntryLink(album)} to revision {archivedVersion.Version}", session);

			return new EntryRevertedContract(album, warnings);
		});
	}

#nullable enable
	public async Task<AlbumForEditForApiContract> UpdateBasicProperties(AlbumForEditForApiContract properties, EntryPictureFileContract? pictureData)
	{
		ParamIs.NotNull(() => properties);

		return await _repository.HandleTransactionAsync(async session =>
		{
			var album = await session.LoadAsync(properties.Id);

			VerifyEntryEdit(album);

			var diff = new AlbumDiff(DoSnapshot(album.ArchivedVersionsManager.GetLatestVersion(), session.OfType<User>().GetLoggedUser(PermissionContext)));

			session.AuditLogger.SysLog($"updating properties for {album}");

			if (album.DiscType != properties.DiscType)
			{
				album.DiscType = properties.DiscType;
				album.UpdateArtistString();
				diff.DiscType.Set();
			}

			diff.Description.Set(album.Description.CopyFrom(properties.Description));

			var parsedBarcodes = properties.Identifiers.Select(Album.ParseBarcode).ToArray();
			var barcodeDiff = album.SyncIdentifiers(parsedBarcodes);
			session.Sync(barcodeDiff);
			if (barcodeDiff.Changed)
			{
				diff.Identifiers.Set();
			}

			if (album.TranslatedName.DefaultLanguage != properties.DefaultNameLanguage)
			{
				album.TranslatedName.DefaultLanguage = properties.DefaultNameLanguage;
				diff.OriginalName.Set();
			}

			var validNames = properties.Names;
			var nameDiff = album.Names.Sync(validNames, album);
			await session.SyncAsync(nameDiff);

			album.Names.UpdateSortNames();

			if (nameDiff.Changed)
				diff.Names.Set();

			var webLinkDiff = WebLink.Sync(album.WebLinks, properties.WebLinks, album);
			session.OfType<AlbumWebLink>().Sync(webLinkDiff);

			if (webLinkDiff.Changed)
				diff.WebLinks.Set();

			var newEvents = properties.OriginalRelease.ReleaseEvents.Select(e => new CreateEventQuery().FindOrCreate(session, PermissionContext, e, album));
			var newOriginalRelease = (properties.OriginalRelease != null ? new AlbumRelease(properties.OriginalRelease, newEvents.ToArray()) : new AlbumRelease());
			newOriginalRelease.ReleaseEvents = newOriginalRelease.ReleaseEvents.DistinctBy(e => e.Id).ToArray();

			if (album.OriginalRelease == null)
				album.OriginalRelease = new AlbumRelease();

			if (!album.OriginalRelease.Equals(newOriginalRelease))
			{
				album.OriginalRelease = newOriginalRelease;
				diff.OriginalRelease.Set();
			}

			// Required because of a bug in NHibernate
			NHibernateUtil.Initialize(album.CoverPictureData);

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				if (pictureData != null)
				{
					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					album.CoverPictureData = new PictureData(parsed);
					album.CoverPictureMime = parsed.Mime;

					pictureData.Id = album.Id;
					pictureData.EntryType = EntryType.Album;
					var thumbGenerator = new ImageThumbGenerator(_imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.AllThumbs);

					diff.Cover.Set();
				}
			}

			if (album.Status != properties.Status)
			{
				album.Status = properties.Status;
				diff.Status.Set();
			}

			var artistGetter = new Func<ArtistContract, Task<Artist>>(artist =>
				session.LoadAsync<Artist>(artist.Id));

			var artistsDiff = await album.SyncArtists(properties.ArtistLinks, artistGetter);
			await session.OfType<ArtistForAlbum>().SyncAsync(artistsDiff);

			if (artistsDiff.Changed)
				diff.Artists.Set();

			var discsDiff = await album.SyncDiscs(properties.Discs);
			await session.OfType<AlbumDiscProperties>().SyncAsync(discsDiff);

			if (discsDiff.Changed)
				diff.Discs.Set();

			var songGetter = new Func<SongInAlbumEditContract, Task<Song>>(async contract =>
			{
				if (contract.SongId != 0)
					return await session.LoadAsync<Song>(contract.SongId);
				else
				{
					var songName = StringHelper.TrimIfNotWhitespace(contract.SongName);

					session.AuditLogger.SysLog($"creating a new song '{songName}' to {album}");

					var song = new Song(new LocalizedString(songName, ContentLanguageSelection.Unspecified));
					await session.SaveAsync(song);

					var songDiff = new SongDiff();
					songDiff.Names.Set();
					var songArtistDiff = await song.SyncArtistsAsync(contract.Artists,
						addedArtistContracts => GetArtistsAsync(session, addedArtistContracts));

					if (songArtistDiff.Changed)
					{
						songDiff.Artists.Set();
						await session.UpdateAsync(song);
					}

					await session.SyncAsync(songArtistDiff);

					var archived = await ArchiveSongAsync(session.OfType<Song>(), song, songDiff, SongArchiveReason.Created,
						$"Created for album '{album.DefaultName.TruncateWithEllipsis(100)}'");

					await session.AuditLogger.AuditLogAsync($"created {_entryLinkFactory.CreateEntryLink(song)} for {_entryLinkFactory.CreateEntryLink(album)}");
					await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), song, EntryEditEvent.Created, archived);

					return song;
				}
			});

			var tracksDiff = await album.SyncSongs(properties.Songs, songGetter,
				(song, artistContracts) => UpdateSongArtistsAsync(session, song, artistContracts));

			await session.OfType<SongInAlbum>().SyncAsync(tracksDiff);

			if (tracksDiff.Changed)
			{
				var add = string.Join(", ", tracksDiff.Added.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));
				var rem = string.Join(", ", tracksDiff.Removed.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));
				var edit = string.Join(", ", tracksDiff.Edited.Select(i => HttpUtility.HtmlEncode(i.SongToStringOrName)));

				var str = $"edited tracks (added: {add}, removed: {rem}, reordered: {edit})"
					.Truncate(300);

				await session.AuditLogger.AuditLogAsync(str);

				diff.Tracks.Set();
			}

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				var picsDiff = album.Pictures.SyncPictures(properties.Pictures, session.OfType<User>().GetLoggedUser(PermissionContext), album.CreatePicture);
				await session.OfType<AlbumPictureFile>().SyncAsync(picsDiff);
				var entryPictureFileThumbGenerator = new ImageThumbGenerator(_pictureFilePersister);
				album.Pictures.GenerateThumbsAndMoveImage(entryPictureFileThumbGenerator, picsDiff.Added, ImageSizes.Original | ImageSizes.Thumb);

				if (picsDiff.Changed)
					diff.Pictures.Set();
			}

			var pvDiff = album.SyncPVs(properties.PVs);
			await session.OfType<PVForAlbum>().SyncAsync(pvDiff);

			if (pvDiff.Changed)
				diff.PVs.Set();

			var logStr = $"updated properties for album {_entryLinkFactory.CreateEntryLink(album)} ({diff.ChangedFieldsString})"
				+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
				.Truncate(400);

			await session.AuditLogger.AuditLogAsync(logStr);

			var archivedAlbum = Archive(session, album, diff, AlbumArchiveReason.PropertiesUpdated, properties.UpdateNotes);
			await session.UpdateAsync(album);

			await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), album, EntryEditEvent.Updated, archivedAlbum);

			var newSongCutoff = TimeSpan.FromHours(1);
			if (artistsDiff.Added.Any() && album.CreateDate >= DateTime.Now - newSongCutoff)
			{
				var addedArtists = artistsDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

				if (addedArtists.Any())
				{
					await _followedArtistNotifier.SendNotificationsAsync(session, album, addedArtists, PermissionContext.LoggedUser);
				}
			}

			return new AlbumForEditForApiContract(album, PermissionContext.LanguagePreference, _imageUrlFactory, PermissionContext);
		});
	}
#nullable disable

	public void UpdatePersonalDescription(int albumId, AlbumDetailsContract data)
	{
		PermissionContext.VerifyLogin();

		HandleTransaction(ctx =>
		{
			var album = ctx.Load(albumId);

			EntryPermissionManager.VerifyAccess(PermissionContext, album, EntryPermissionManager.CanEditPersonalDescription);

			album.PersonalDescriptionText = data.PersonalDescriptionText;
			album.PersonalDescriptionAuthorId = data.PersonalDescriptionAuthor?.Id;

			ctx.Update(album);
			ctx.AuditLogger.AuditLog($"updated personal description for {_entryLinkFactory.CreateEntryLink(album)}");
		});
	}

	public void DeleteComment(int commentId) => HandleTransaction(ctx => Comments(ctx).Delete(commentId));

	public IEnumerable<int> GetIds()
	{
		return HandleQuery(ctx =>
		{
			return ctx.Query()
				.Where(a => !a.Deleted)
				.Select(v => v.Id)
				.ToArray();
		});
	}

	public EntryIdAndVersionContract[] GetVersions()
	{
		return HandleQuery(ctx =>
		{
			return ctx.Query()
				.Where(a => !a.Deleted)
				.Select(a => new { a.Id, a.Version })
				.ToArray()
				.Select(v => new EntryIdAndVersionContract(v.Id, v.Version))
				.ToArray();
		});
	}

	public void PostEditComment(int commentId, CommentForApiContract contract) => HandleTransaction(ctx => Comments(ctx).Update(commentId, contract));

#nullable enable
	public EntryWithArchivedVersionsForApiContract<AlbumForApiContract> GetAlbumWithArchivedVersionsForApi(int albumId)
	{
		return HandleQuery(session =>
		{
			var album = session.Load<Album>(albumId);
			return EntryWithArchivedVersionsForApiContract.Create(
				entry: new AlbumForApiContract(album, PermissionContext.LanguagePreference, PermissionContext, thumbPersister: null, fields: AlbumOptionalFields.None),
				versions: album.ArchivedVersionsManager.Versions
					.Select(a => ArchivedObjectVersionForApiContract.FromAlbum(a, _userIconFactory))
					.ToArray()
			);
		});
	}

	public ArchivedAlbumVersionDetailsForApiContract GetVersionDetailsForApi(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedAlbumVersionDetailsForApiContract(
				archived: session.Load<ArchivedAlbumVersion>(id),
				comparedVersion: comparedVersionId != 0 ? session.Load<ArchivedAlbumVersion>(comparedVersionId) : null,
				permissionContext: PermissionContext,
				userIconFactory: _userIconFactory
			);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}
#nullable disable
}
