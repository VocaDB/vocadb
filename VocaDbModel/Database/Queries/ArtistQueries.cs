#nullable disable

using System.Runtime.Caching;
using NHibernate;
using NLog;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
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
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.EntryValidators;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries;

/// <summary>
/// Database queries related to <see cref="Artist"/>.
/// </summary>
public class ArtistQueries : QueriesBase<IArtistRepository, Artist>
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private readonly ObjectCache _cache;
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly IEnumTranslations _enumTranslations;
	private readonly IEntryThumbPersister _imagePersister;
	private readonly IAggregatedEntryImageUrlFactory _imageUrlFactory;
	private readonly IEntryPictureFilePersister _pictureFilePersister;
	private readonly IUserIconFactory _userIconFactory;
	private readonly IDiscordWebhookNotifier _discordWebhookNotifier;

	class CachedAdvancedArtistStatsContract
	{
		public TopStatContract<TranslatedArtistContract>[] TopVocaloids { get; set; }

		public TopStatContract<string>[] TopLanguages { get; init; }
	}

#nullable enable
	/// <summary>
	/// Advanced/less important statistics.
	/// These are cached for 24 hours.
	/// </summary>
	private AdvancedArtistStatsContract? GetAdvancedStats(IDatabaseContext<Artist> ctx, Artist artist)
	{
		if (artist.ArtistType != ArtistType.Producer && artist.ArtistType != ArtistType.CoverArtist)
			return null;

		var key = $"ArtistQueries.AdvancedArtistStatsContract.{artist.Id}";

		var cached = _cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(24), () =>
		{
			var topVocaloids = new ArtistRelationsQuery(ctx, LanguagePreference, PermissionContext, _cache, _imageUrlFactory).GetTopVoicebanks(artist);
			var topLanguages = new ArtistRelationsQuery(ctx, LanguagePreference, PermissionContext, _cache, _imageUrlFactory).GetTopLanguages(artist);

			return new CachedAdvancedArtistStatsContract
			{
				TopVocaloids = topVocaloids,
				TopLanguages = topLanguages
			};
		});

		return new AdvancedArtistStatsContract
		{
			TopVocaloids = cached.TopVocaloids.Select(v => new TopStatContract<ArtistContract>
			{
				Data = new ArtistContract(v.Data, LanguagePreference),
				Count = v.Count
			}).ToArray(),
			TopLanguages = cached.TopLanguages
		};
	}

	/// <summary>
	/// Stats related to logged in user.
	/// These stats are cached for 1 hour.
	/// </summary>
	private PersonalArtistStatsContract? GetPersonalArtistStats(IDatabaseContext<Artist> ctx, Artist artist)
	{
		if (!PermissionContext.IsLoggedIn)
			return null;

		var key = $"ArtistQueries.PersonalArtistStatsContract.{artist.Id}.{PermissionContext.LoggedUserId}";
		return _cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () =>
		{
			return new PersonalArtistStatsContract
			{
				SongRatingCount = ctx.OfType<FavoriteSongForUser>()
					.Query()
					.Count(f => f.User.Id == PermissionContext.LoggedUserId && f.Song.AllArtists.Any(a => a.Artist.Id == artist.Id))
			};
		});
	}

	/// <summary>
	/// Stats shared for all users. These are cached for 1 hour.
	/// </summary>
	private SharedArtistStatsContract GetSharedArtistStats(IDatabaseContext<Artist> ctx, Artist artist)
	{
		var key = $"ArtistQueries.SharedArtistStatsContract.{artist.Id}";
		return _cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(1), () =>
		{
			try
			{
				var stats = ctx.Query()
					.Where(a => a.Id == artist.Id)
					.Select(a => new
					{
						FollowCount = a.Users.Count,
						AlbumCount = a.AllAlbums.Count(l => !l.Album.Deleted),
						RatedAlbumCount = a.AllAlbums.Count(l => !l.Album.Deleted && l.Album.RatingCount > 0),
						SongCount = a.AllSongs.Count(s => !s.Song.Deleted),
						RatedSongCount = a.AllSongs.Count(s => !s.Song.Deleted && s.Song.RatingScore > 0),
						AlbumRatingsTotalCount = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingCount) : 0,
						AlbumRatingsTotalSum = a.AllAlbums.Any() ? a.AllAlbums.Sum(l => l.Album.RatingTotal) : 0,
						EventCount = a.AllEvents.Count(e => !e.ReleaseEvent.Deleted)
					})
					.FirstOrDefault();

				return new SharedArtistStatsContract
				{
					AlbumCount = stats.AlbumCount,
					FollowerCount = stats.FollowCount,
					RatedAlbumCount = stats.RatedAlbumCount,
					SongCount = stats.SongCount,
					RatedSongCount = stats.RatedSongCount,
					AlbumRatingAverage = (stats.AlbumRatingsTotalCount > 0 ? Math.Round(stats.AlbumRatingsTotalSum / (double)stats.AlbumRatingsTotalCount, 2) : 0),
					EventCount = stats.EventCount
				};
			}
			catch (HibernateException x)
			{
				// TODO: Loading of stats timeouts sometimes. Since they're not essential we can accept returning only partial stats.
				// However, this should be fixed by tuning the queries further.
				s_log.Error(x, "Unable to load shared artist stats");
				return new SharedArtistStatsContract();
			}
		});
	}
#nullable disable

	private ArtistMergeRecord GetMergeRecord(IDatabaseContext<Artist> session, int sourceId)
	{
		return session.OfType<ArtistMergeRecord>().Query().FirstOrDefault(s => s.Source == sourceId);
	}

	public ArtistQueries(
		IArtistRepository repository,
		IUserPermissionContext permissionContext,
		IEntryLinkFactory entryLinkFactory,
		IEntryThumbPersister imagePersister,
		IEntryPictureFilePersister pictureFilePersister,
		ObjectCache cache,
		IUserIconFactory userIconFactory,
		IEnumTranslations enumTranslations,
		IAggregatedEntryImageUrlFactory imageUrlFactory,
		IDiscordWebhookNotifier discordWebhookNotifier
	)
		: base(repository, permissionContext)
	{
		_entryLinkFactory = entryLinkFactory;
		_imagePersister = imagePersister;
		_pictureFilePersister = pictureFilePersister;
		_cache = cache;
		_userIconFactory = userIconFactory;
		_enumTranslations = enumTranslations;
		_imageUrlFactory = imageUrlFactory;
		_discordWebhookNotifier = discordWebhookNotifier;
	}

	public ICommentQueries Comments(IDatabaseContext<Artist> ctx)
	{
		return new CommentQueries<ArtistComment, Artist>(ctx.OfType<ArtistComment>(), PermissionContext, _userIconFactory, _entryLinkFactory);
	}

	public async Task<ArchivedArtistVersion> ArchiveAsync(IDatabaseContext<Artist> ctx, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "")
	{
		ctx.AuditLogger.SysLog("Archiving " + artist);

		var agentLoginData = await ctx.CreateAgentLoginDataAsync(PermissionContext);
		var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
		await ctx.SaveAsync(archived);
		return archived;
	}

	public Task<ArchivedArtistVersion> ArchiveAsync(IDatabaseContext<Artist> ctx, Artist artist, ArtistArchiveReason reason, string notes = "")
	{
		return ArchiveAsync(ctx, artist, new ArtistDiff(), reason, notes);
	}

#nullable enable
	public async Task<ArtistContract> Create(CreateArtistForApiContract contract)
	{
		ParamIs.NotNull(() => contract);

		if (contract.Names == null || !contract.Names.Any())
			throw new ArgumentException("Artist needs at least one name", "contract");

		VerifyManageDatabase();

		var diff = new ArtistDiff();
		diff.Names.Set();

		return await _repository.HandleTransactionAsync(async ctx =>
		{
			ctx.AuditLogger.SysLog($"creating a new artist with name '{contract.Names.First().Value}'");

			var artist = new Artist
			{
				ArtistType = contract.ArtistType,
				Description = new EnglishTranslatedString(contract.Description.Trim())
			};

			artist.Names.Init(contract.Names, artist);

			if (contract.WebLink != null)
			{
				artist.CreateWebLink(contract.WebLink.Description, contract.WebLink.Url, contract.WebLink.Category, contract.WebLink.Disabled);
				diff.WebLinks.Set();
			}

			artist.Status = (contract.Draft || !(new ArtistValidator().IsValid(artist))) ? EntryStatus.Draft : EntryStatus.Finished;

			await ctx.SaveAsync(artist);

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				if (contract.PictureData != null)
				{
					var pictureData = contract.PictureData;
					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(_imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Picture.Set();
				}
			}

			var archived = await ArchiveAsync(ctx, artist, diff, ArtistArchiveReason.Created);
			await ctx.UpdateAsync(artist);

			await ctx.AuditLogger.AuditLogAsync($"created artist {_entryLinkFactory.CreateEntryLink(artist)} ({artist.ArtistType})");
			await AddEntryEditedEntryAsync(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Created, archived);

			return new ArtistContract(artist, PermissionContext.LanguagePreference);
		});
	}

	public CommentForApiContract CreateComment(int artistId, CommentForApiContract contract)
	{
		return HandleTransaction(ctx => Comments(ctx).Create(artistId, contract));
	}

	public Task<(bool created, int reportId)> CreateReport(int artistId, ArtistReportType reportType, string hostname, string notes, int? versionNumber)
	{
		ParamIs.NotNull(() => hostname);
		ParamIs.NotNull(() => notes);

		return HandleTransactionAsync(ctx =>
		{
			return new Service.Queries.EntryReportQueries().CreateReport(
				ctx,
				PermissionContext,
				_entryLinkFactory,
				(artist, reporter, notesTruncated) => new ArtistReport(artist, reportType, reporter, hostname, notesTruncated, versionNumber),
				() => reportType != ArtistReportType.Other ? _enumTranslations.ArtistReportTypeNames[reportType] : null,
				artistId,
				reportType,
				hostname,
				notes,
				_discordWebhookNotifier,
				ArtistReport.ReportTypesWithRequiredNotes,
				reportType != ArtistReportType.OwnershipClaim
			);
		});
	}
#nullable disable

	public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName, string url)
	{
		var names = anyName.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToArray();

		if (!names.Any() && string.IsNullOrEmpty(url))
			return Array.Empty<EntryRefWithCommonPropertiesContract>();

		return HandleQuery(session =>
		{
			// TODO: moved Distinct after ToArray to work around NH bug
			var nameMatches = (names.Any() ? session.Query<ArtistName>()
				.Where(n => names.Contains(n.Value) && !n.Artist.Deleted)
				.OrderBy(n => n.Artist)
				.Select(n => n.Artist)
				.Take(10)
				.ToArray()
				.Distinct() : Array.Empty<Artist>());

			var linkMatches = !string.IsNullOrWhiteSpace(url) ? session.Query<ArtistWebLink>()
				.Where(w => !w.Entry.Deleted)
				.WhereUrlIs(url, WebLinkVariationTypes.IgnoreScheme)
				.Select(w => w.Entry)
				.Take(10)
				.ToArray()
				.Distinct() : Array.Empty<Artist>();

			return nameMatches.Union(linkMatches)
				.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
				.ToArray();
		});
	}

	public T Get<T>(int id, Func<Artist, T> fac)
	{
		return HandleQuery(ctx => fac(ctx.Load(id)));
	}

#nullable enable
	public ArtistForEditForApiContract GetArtistForEdit(int id)
	{
		return HandleQuery(session => new ArtistForEditForApiContract(
			artist: session.Load<Artist>(id),
			languagePreference: PermissionContext.LanguagePreference,
			imageStore: _imageUrlFactory,
			permissionContext: PermissionContext
		));
	}
#nullable disable

	public CommentForApiContract[] GetComments(int artistId)
	{
		return HandleQuery(ctx => ctx.Load(artistId).Comments.Select(c => new CommentForApiContract(c, _userIconFactory, true)).ToArray());
	}

#nullable enable
	public ArtistDetailsContract GetDetails(int id, string hostname)
	{
		return HandleQuery(session =>
		{
			var artist = session.Load(id);

			var contract = new ArtistDetailsContract(artist, LanguagePreference, PermissionContext, _imageUrlFactory, _userIconFactory,
				new EntryTypeTags(session).GetTag(EntryType.Artist, artist.ArtistType))
			{
				CommentCount = Comments(session).GetCount(id),
				SharedStats = GetSharedArtistStats(session, artist),
				PersonalStats = GetPersonalArtistStats(session, artist),
				AdvancedStats = GetAdvancedStats(session, artist)
			};

			if (PermissionContext.IsLoggedIn)
			{
				var subscription = session.OfType<ArtistForUser>()
					.Query()
					.FirstOrDefault(s => s.Artist.Id == id && s.User.Id == PermissionContext.LoggedUserId);

				if (subscription != null)
				{
					contract.IsAdded = true;
					contract.EmailNotifications = subscription.EmailNotifications;
					contract.SiteNotifications = subscription.SiteNotifications;
				}
			}

			var relations = (new ArtistRelationsQuery(session, LanguagePreference, PermissionContext, _cache, _imageUrlFactory)).GetRelations(artist, ArtistRelationsFields.All);
			contract.LatestAlbums = relations.LatestAlbums;
			contract.TopAlbums = relations.PopularAlbums;
			contract.LatestSongs = relations.LatestSongs;
			contract.TopSongs = relations.PopularSongs;
			contract.LatestEvents = relations.LatestEvents;

			// If song and album counts are out of date and we know there's more albums/songs than that, update counts.
			contract.SharedStats.AlbumCount = Math.Max(contract.SharedStats.AlbumCount, contract.LatestAlbums.Length + contract.TopAlbums.Length);
			contract.SharedStats.SongCount = Math.Max(contract.SharedStats.SongCount, contract.LatestSongs.Length + contract.TopSongs.Length);

			contract.LatestComments = Comments(session).GetList(id, 3);

			if (artist.Deleted)
			{
				var mergeEntry = GetMergeRecord(session, id);
				contract.MergedTo = (mergeEntry != null ? new ArtistContract(mergeEntry.Target, LanguagePreference) : null);
			}

			new CreateEntryHitQuery().CreateHit(session, artist, hostname, PermissionContext, (a, agent) => new ArtistHit(a, agent));

			return contract;
		});
	}

	public ArtistDetailsForApiContract GetDetailsForApi(int id, string hostname)
	{
		return HandleQuery(session =>
		{
			var artist = session.Load(id);

			var contract = new ArtistDetailsForApiContract(
				artist: artist,
				languagePreference: LanguagePreference,
				userIconFactory: _userIconFactory,
				userContext: PermissionContext,
				imageStore: _imageUrlFactory,
				artistTypeTag: new EntryTypeTags(session).GetTag(EntryType.Artist, artist.ArtistType)
			)
			{
				CommentCount = Comments(session).GetCount(id),
				SharedStats = GetSharedArtistStats(session, artist),
				PersonalStats = GetPersonalArtistStats(session, artist),
				AdvancedStats = GetAdvancedStats(session, artist),
			};

			if (PermissionContext.IsLoggedIn)
			{
				var subscription = session.OfType<ArtistForUser>()
					.Query()
					.FirstOrDefault(s => s.Artist.Id == id && s.User.Id == PermissionContext.LoggedUserId);

				if (subscription is not null)
				{
					contract.IsAdded = true;
					contract.EmailNotifications = subscription.EmailNotifications;
					contract.SiteNotifications = subscription.SiteNotifications;
				}
			}

			var relations = new ArtistRelationsQuery(session, LanguagePreference, PermissionContext, _cache, _imageUrlFactory).GetRelations(artist, ArtistRelationsFields.All);
			contract.LatestAlbums = relations.LatestAlbums;
			contract.TopAlbums = relations.PopularAlbums;
			contract.LatestSongs = relations.LatestSongs;
			contract.TopSongs = relations.PopularSongs;
			contract.LatestEvents = relations.LatestEvents;

			// If song and album counts are out of date and we know there's more albums/songs than that, update counts.
			contract.SharedStats.AlbumCount = Math.Max(contract.SharedStats.AlbumCount, contract.LatestAlbums.Length + contract.TopAlbums.Length);
			contract.SharedStats.SongCount = Math.Max(contract.SharedStats.SongCount, contract.LatestSongs.Length + contract.TopSongs.Length);

			contract.LatestComments = Comments(session).GetList(entryId: id, count: 3);

			if (artist.Deleted)
			{
				var mergeEntry = GetMergeRecord(session, id);
				contract.MergedTo = mergeEntry is not null
					? new ArtistForApiContract(
						artist: mergeEntry.Target,
						languagePreference: LanguagePreference,
						PermissionContext,
						thumbPersister: null,
						includedFields: ArtistOptionalFields.None
					)
					: null;
			}

			new CreateEntryHitQuery().CreateHit(session, artist, hostname, PermissionContext, (a, agent) => new ArtistHit(a, agent));

			return contract;
		});
	}
#nullable disable

	public T GetWithMergeRecord<T>(int id, Func<Artist, ArtistMergeRecord, IDatabaseContext<Artist>, T> fac)
	{
		return HandleQuery(session =>
		{
			var artist = session.Load(id);
			return fac(artist, (artist.Deleted ? GetMergeRecord(session, id) : null), session);
		});
	}

	public EntryForPictureDisplayContract GetPictureThumb(int artistId)
	{
		var size = ImageSize.Thumb;

		return _repository.HandleQuery(ctx =>
		{
			var artist = ctx.Load(artistId);

			if (artist.Picture == null || string.IsNullOrEmpty(artist.PictureMime))
				return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference);

			var data = artist.Thumb;

			if (_imagePersister.HasImage(data, size))
			{
				var bytes = _imagePersister.ReadBytes(data, size);
				return EntryForPictureDisplayContract.Create(artist, data.Mime, bytes, PermissionContext.LanguagePreference);
			}

			return EntryForPictureDisplayContract.Create(artist, PermissionContext.LanguagePreference);
		});
	}

	public TagUsageForApiContract[] GetTagSuggestions(int artistId)
	{
		return _repository.HandleQuery(ctx =>
		{
			var artist = ctx.Load<Artist>(artistId);
			var artistTags = artist.Tags.Tags.Select(t => t.Id);

			var albumUsages = ctx.Query<AlbumTagUsage>()
				.Where(u => !artistTags.Contains(u.Tag.Id)
					&& !u.Tag.Deleted
					&& !u.Tag.HideFromSuggestions
					&& u.Entry.AllArtists.Any(a => !a.IsSupport && a.Artist.Id == artistId))
				.WhereTagHasTarget(TagTargetTypes.Artist)
				.GroupBy(t => t.Tag.Id)
				.Select(t => new { TagId = t.Key, Count = t.Count() })
				.Where(t => t.Count > 1)
				.OrderByDescending(t => t.Count)
				.Take(3)
				.ToArray();

			var songUsages = ctx.Query<SongTagUsage>()
				.Where(u => !artistTags.Contains(u.Tag.Id)
					&& !u.Tag.Deleted
					&& !u.Tag.HideFromSuggestions
					&& u.Entry.AllArtists.Any(a => !a.IsSupport && a.Artist.Id == artistId))
				.WhereTagHasTarget(TagTargetTypes.Artist)
				.GroupBy(t => t.Tag.Id)
				.Select(t => new { TagId = t.Key, Count = t.Count() })
				.Where(t => t.Count > 1)
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

	/// <summary>
	/// Reverts an album to an earlier archived version.
	/// </summary>
	/// <param name="archivedArtistVersionId">Id of the archived version to be restored.</param>
	/// <returns>Result of the revert operation, with possible warnings if any. Cannot be null.</returns>
	/// <remarks>Requires the RestoreRevisions permission.</remarks>
	public async Task<EntryRevertedContract> RevertToVersion(int archivedArtistVersionId)
	{
		PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

		return await HandleTransactionAsync(async session =>
		{
			var archivedVersion = await session.LoadAsync<ArchivedArtistVersion>(archivedArtistVersionId);

			if (archivedVersion.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			var artist = archivedVersion.Artist;

			session.AuditLogger.SysLog("reverting " + artist + " to version " + archivedVersion.Version);

			var fullProperties = ArchivedArtistContract.GetAllProperties(archivedVersion);
			var warnings = new List<string>();
			var diff = new ArtistDiff();

			artist.ArtistType = fullProperties.ArtistType;
			artist.Description.Original = fullProperties.Description;
			artist.Description.English = fullProperties.DescriptionEng ?? string.Empty;
			artist.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;
			artist.BaseVoicebank = DatabaseContextHelper.RestoreWeakRootEntityRef(
				session: session,
				warnings: warnings,
				objRef: fullProperties.BaseVoicebank
			);

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);

				if (versionWithPic != null)
				{
					artist.Picture = versionWithPic.Picture;
					artist.PictureMime = versionWithPic.PictureMime;

					if (versionWithPic.Picture != null)
					{
						var thumbGenerator = new ImageThumbGenerator(_imagePersister);
						using var stream = new MemoryStream(versionWithPic.Picture.Bytes);
						var thumb = new EntryThumb(artist, versionWithPic.PictureMime, ImagePurpose.Main);
						thumbGenerator.GenerateThumbsAndMoveImage(stream, thumb, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);
					}
				}
				else
				{
					artist.Picture = null;
					artist.PictureMime = null;
				}

				// Assume picture was changed if there's a version between the current version and the restored version where the picture was changed.
				diff.Picture.Set(!Equals(artist.ArchivedVersionsManager.GetLatestVersionWithField(ArtistEditableFields.Picture, artist.Version), versionWithPic));
			}

			// Groups
			DatabaseContextHelper.RestoreObjectRefs(
				session: session,
				warnings: warnings,
				existing: artist.AllGroups,
				objRefs: fullProperties.Groups,
				equality: (a1, a2) => a1.Parent.Id == a2.Id,
				createEntryFunc: (grp, grpRef) => !artist.HasGroup(grp) ? artist.AddGroup(grp, grpRef.LinkType) : null,
				deleteFunc: groupForArtist => groupForArtist.Delete()
			);

			// Names
			if (fullProperties.Names != null)
			{
				var nameDiff = artist.Names.SyncByContent(fullProperties.Names, artist);
				await session.SyncAsync(nameDiff);
			}

			// Weblinks
			if (fullProperties.WebLinks != null)
			{
				var webLinkDiff = WebLink.SyncByValue(
					oldLinks: artist.WebLinks,
					newLinks: fullProperties.WebLinks,
					webLinkFactory: artist
				);
				await session.SyncAsync(webLinkDiff);
			}

			await ArchiveAsync(session, artist, diff, ArtistArchiveReason.Reverted, $"Reverted to version {archivedVersion.Version}");
			await AuditLogAsync($"reverted {_entryLinkFactory.CreateEntryLink(artist)} to revision {archivedVersion.Version}", session);

			return new EntryRevertedContract(artist, warnings);
		});
	}

#nullable enable
	public async Task<int> Update(ArtistForEditForApiContract properties, EntryPictureFileContract? pictureData, IUserPermissionContext permissionContext)
	{
		ParamIs.NotNull(() => properties);
		ParamIs.NotNull(() => permissionContext);

		return await _repository.HandleTransactionAsync(async ctx =>
		{
			var artist = await ctx.LoadAsync(properties.Id);

			VerifyEntryEdit(artist);

			var diff = new ArtistDiff(DoSnapshot(artist.GetLatestVersion(), await ctx.OfType<User>().GetLoggedUserAsync(permissionContext)));

			ctx.AuditLogger.SysLog($"updating properties for {artist}");

			if (artist.ArtistType != properties.ArtistType)
			{
				artist.ArtistType = properties.ArtistType;
				diff.ArtistType.Set();
			}

			diff.Description.Set(artist.Description.CopyFrom(properties.Description));

			if (artist.TranslatedName.DefaultLanguage != properties.DefaultNameLanguage)
			{
				artist.TranslatedName.DefaultLanguage = properties.DefaultNameLanguage;
				diff.OriginalName.Set();
			}

			// Required because of a bug in NHibernate
			NHibernateUtil.Initialize(artist.Picture);

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				if (pictureData != null)
				{
					var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
					artist.Picture = new PictureData(parsed);
					artist.PictureMime = parsed.Mime;

					pictureData.Id = artist.Id;
					pictureData.EntryType = EntryType.Artist;
					var thumbGenerator = new ImageThumbGenerator(_imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ImageSizes.Thumb | ImageSizes.SmallThumb | ImageSizes.TinyThumb);

					diff.Picture.Set();
				}
			}

			if (artist.Status != properties.Status)
			{
				artist.Status = properties.Status;
				diff.Status.Set();
			}

			var nameDiff = artist.Names.Sync(properties.Names, artist);
			await ctx.OfType<ArtistName>().SyncAsync(nameDiff);

			if (nameDiff.Changed)
				diff.Names.Set();

			if (!artist.BaseVoicebank.NullSafeIdEquals(properties.BaseVoicebank))
			{
				var newBase = await ctx.NullSafeLoadAsync(properties.BaseVoicebank);

				if (artist.IsValidBaseVoicebank(newBase))
				{
					diff.BaseVoicebank.Set();
					artist.SetBaseVoicebank(await ctx.NullSafeLoadAsync(properties.BaseVoicebank));
				}
			}

			if (!artist.ReleaseDate.Equals(properties.ReleaseDate))
			{
				artist.ReleaseDate = properties.ReleaseDate;
				diff.ReleaseDate.Set();
			}

			var webLinkDiff = WebLink.Sync(artist.WebLinks, properties.WebLinks, artist);
			await ctx.OfType<ArtistWebLink>().SyncAsync(webLinkDiff);

			if (webLinkDiff.Changed)
				diff.WebLinks.Set();

			var newCultureCodes = properties.CultureCodes.Select(c => new OptionalCultureCode(c)).ToArray();
			if (!artist.CultureCodes.SequenceEqual(newCultureCodes))
			{
				artist.CultureCodes = newCultureCodes;
				diff.CultureCodes.Set();
			}

			if (diff.ArtistType.IsChanged || diff.Names.IsChanged || diff.OriginalName.IsChanged)
			{
				foreach (var song in artist.Songs)
				{
					song.Song.UpdateArtistString();
					await ctx.UpdateAsync(song);
				}
			}

			var newGroups = properties.Groups
				.Select(g => new ArtistForArtistContract { Parent = g.Parent, LinkType = ArtistLinkType.Group })
				.Concat(new[] {
					new ArtistForArtistContract { Parent = properties.Illustrator, LinkType = ArtistLinkType.Illustrator },
					new ArtistForArtistContract { Parent = properties.VoiceProvider, LinkType = ArtistLinkType.VoiceProvider }
				})
				.Concat(properties.AssociatedArtists)
				.Where(a => a.Parent != null && ArtistHelper.CanHaveRelatedArtists(artist.ArtistType, a.LinkType, LinkDirection.ManyToOne))
				.ToArray();

			var groupsDiff = CollectionHelper.Diff(artist.Groups, newGroups, (i, i2) => (i.Parent.Id == i2.Parent.Id && i.LinkType == i2.LinkType));

			foreach (var grp in groupsDiff.Removed)
			{
				grp.Delete();
				await ctx.DeleteAsync(grp);
			}

			foreach (var grp in groupsDiff.Added)
			{
				var link = artist.AddGroup(ctx.Load(grp.Parent.Id), grp.LinkType);
				await ctx.SaveAsync(link);
			}

			if (groupsDiff.Changed)
				diff.Groups.Set();

			if (PermissionContext.HasPermission(PermissionToken.ViewCoverArtImages))
			{
				var picsDiff = artist.Pictures.SyncPictures(properties.Pictures, ctx.OfType<User>().GetLoggedUser(permissionContext), artist.CreatePicture);
				ctx.OfType<ArtistPictureFile>().Sync(picsDiff);
				var entryPictureFileThumbGenerator = new ImageThumbGenerator(_pictureFilePersister);
				artist.Pictures.GenerateThumbsAndMoveImage(entryPictureFileThumbGenerator, picsDiff.Added, ImageSizes.Original | ImageSizes.Thumb);

				if (picsDiff.Changed)
					diff.Pictures.Set();
			}

			var logStr = $"updated properties for artist {_entryLinkFactory.CreateEntryLink(artist)} ({diff.ChangedFieldsString})"
				+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
				.Truncate(400);

			var archived = await ArchiveAsync(ctx, artist, diff, ArtistArchiveReason.PropertiesUpdated, properties.UpdateNotes);
			await ctx.UpdateAsync(artist);

			await ctx.AuditLogger.AuditLogAsync(logStr);
			await AddEntryEditedEntryAsync(ctx.OfType<ActivityEntry>(), artist, EntryEditEvent.Updated, archived);

			return artist.Id;
		});
	}
#nullable disable

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
				.Select(a => new EntryIdAndVersionContract { Id = a.Id, Version = a.Version })
				.ToArray();
		});
	}

	public void PostEditComment(int commentId, CommentForApiContract contract) => HandleTransaction(ctx => Comments(ctx).Update(commentId, contract));

#nullable enable
	public EntryWithArchivedVersionsForApiContract<ArtistForApiContract> GetArtistWithArchivedVersionsForApi(int artistId)
	{
		return HandleQuery(session =>
		{
			var artist = session.Load<Artist>(artistId);
			return EntryWithArchivedVersionsForApiContract.Create(
				entry: new ArtistForApiContract(
					artist,
					PermissionContext.LanguagePreference,
					PermissionContext,
					thumbPersister: null,
					includedFields: ArtistOptionalFields.None
				),
				versions: artist.ArchivedVersionsManager.Versions
					.Select(a => ArchivedObjectVersionForApiContract.FromArtist(a, _userIconFactory))
					.ToArray()
			);
		});
	}

	public ArchivedArtistVersionDetailsForApiContract GetVersionDetailsForApi(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedArtistVersionDetailsForApiContract(
				archived: session.Load<ArchivedArtistVersion>(id),
				comparedVersion: comparedVersionId != 0 ? session.Load<ArchivedArtistVersion>(comparedVersionId) : null,
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
