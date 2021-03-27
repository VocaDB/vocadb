#nullable disable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
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
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
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
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Database.Queries
{
	/// <summary>
	/// Database queries related to <see cref="Song"/>.
	/// </summary>
	public class SongQueries : QueriesBase<ISongRepository, Song>
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly ObjectCache _cache;
		private readonly VdbConfigManager _config;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;
		private readonly IEnumTranslations _enumTranslations;
		private readonly IFollowedArtistNotifier _followedArtistNotifier;
		private readonly ILanguageDetector _languageDetector;
		private readonly IUserMessageMailer _mailer;
		private readonly IPVParser _pvParser;
		private readonly TagMapper _tagMapper = new();
		private readonly IUserIconFactory _userIconFactory;

		private void AddSongHit(IDatabaseContext<Song> session, Song song, string hostname)
		{
			new CreateEntryHitQuery().CreateHit(session, song, hostname, PermissionContext, (s, agent) => new SongHit(s, agent));
		}

		private Tag[] AddTagsFromPV(VideoUrlParseResult pvResult, Song song, IDatabaseContext<Song> ctx)
		{
			if (pvResult.Tags == null || !pvResult.Tags.Any())
				return new Tag[0];

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var tags = MapTags(ctx, pvResult.Tags);

			foreach (var tag in tags)
			{
				if (_tagMapper.TagIsRedundantForSong(song.SongType, tag.Id, new EntryTypeTags(ctx)))
				{
					continue;
				}

				var usage = song.AddTag(tag);

				// Only add the vote if the tag usage isn't added yet.
				if (usage.IsNew)
				{
					ctx.Save(usage.Result);

					var vote = usage.Result.CreateVote(user);
					ctx.Save(vote);

					ctx.Update(usage.Result.Tag);
				}
			}

			return tags;
		}

		private Artist GetArtist(string name, IDatabaseContext<PVForSong> ctx, ArtistType[] preferredArtistTypes)
		{
			if (preferredArtistTypes != null && preferredArtistTypes.Any())
			{
				var preferredArtist = ctx.OfType<ArtistName>()
					.Query()
					.Where(n => n.Value == name && !n.Artist.Deleted && preferredArtistTypes.Contains(n.Artist.ArtistType))
					.Select(n => n.Artist)
					.FirstOrDefault();

				if (preferredArtist != null)
					return preferredArtist;
			}

			return ctx.OfType<ArtistName>()
				.Query()
				.Where(n => n.Value == name && !n.Artist.Deleted)
				.Select(n => n.Artist)
				.FirstOrDefault();
		}

		private SongMergeRecord GetMergeRecord(IDatabaseContext<Song> session, int sourceId)
		{
			return session.Query<SongMergeRecord>().FirstOrDefault(s => s.Source == sourceId);
		}

		private Song[] GetSongSuggestions(IDatabaseContext<Song> ctx, Song song)
		{
			var cacheKey = $"GetSongSuggestions.{song.Id}";

			var songIds = _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(24), () =>
			{
				var related = new RelatedSongsQuery(ctx).GetLikeMatches(song, new[] { song.Id }, 4);
				return related;
			});

			if (!songIds.Any())
				return new Song[0];

			return ctx.Query()
				.Where(s => songIds.Contains(s.Id))
				.ToArray();
		}

		private SongListBaseContract[] GetSongPools(IDatabaseContext<Song> ctx, int songId)
		{
			var cacheKey = $"GetSongPools.{songId}";
			return _cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(1), () =>
			{
				var lists = ctx
					.Query<SongList>()
					.Where(s => s.FeaturedCategory == SongListFeaturedCategory.Pools && s.AllSongs.Any(l => l.Song.Id == songId))
					.OrderBy(s => s.Name)
					.Take(3)
					.ToArray();

				return lists.Select(s => new SongListBaseContract(s)).ToArray();
			});
		}

		private Tag GetSongTypeTag(IDatabaseContext ctx, SongType songType)
		{
			return new EntryTypeTags(ctx).GetTag(EntryType.Song, songType);
		}

		private Tag[] MapTags(IDatabaseContext ctx, string[] nicoTags)
		{
			return new TagMapper().MapTags(ctx, nicoTags);
		}

		private Task<Tag[]> MapTagsAsync(IDatabaseContext ctx, string[] nicoTags)
		{
			return new TagMapper().MapTagsAsync(ctx, nicoTags);
		}

		private bool HasCoverTag(IDatabaseContext<PVForSong> ctx, VideoUrlParseResult res)
		{
			// Tag mappings are only supported for nico for now.
			if (res.Service != PVService.NicoNicoDouga)
				return false;

			var coverTagId = new EntryTypeTags(ctx).Cover;

			if (coverTagId == 0)
				return false;

			var coverSourceTags = ctx.Query<TagMapping>().Where(t => t.Tag.Id == coverTagId).Select(t => t.SourceTag).ToArray();
			return coverSourceTags.Any(coverSourceTag => res.Tags.Contains(coverSourceTag, StringComparer.InvariantCultureIgnoreCase));
		}

		private string[] GetUserProfileUrls(VideoUrlParseResult res)
		{
			var profileUrls = VideoServiceHelper.Services[res.Service]
				.GetUserProfileUrls(res.AuthorId)
				.WhereIsNotNullOrEmpty()
				.ToArray();

			return profileUrls;
		}

		private NicoTitleParseResult ParseNicoPV(IDatabaseContext<PVForSong> ctx, VideoUrlParseResult res)
		{
			if (res == null || !res.IsOk)
				return null;

			var titleParseResult = NicoHelper.ParseTitle(res.Title, a => GetArtist(a, ctx, AppConfig.PreferredNicoArtistTypes));

			if (!string.IsNullOrEmpty(titleParseResult.Title))
				titleParseResult.TitleLanguage = _languageDetector.Detect(titleParseResult.Title, ContentLanguageSelection.Unspecified);

			if (titleParseResult.SongType == SongType.Unspecified)
			{
				titleParseResult.SongType = HasCoverTag(ctx, res) ? SongType.Cover : SongType.Original;
			}

			if (!string.IsNullOrEmpty(res.AuthorId))
			{
				var authorPages = GetUserProfileUrls(res);

				if (authorPages.Any())
				{
					var author = ctx.OfType<ArtistWebLink>().Query()
						.Where(w => authorPages.Contains(w.Url) && !w.Entry.Deleted)
						.Select(w => w.Entry)
						.FirstOrDefault();

					if (author != null && !titleParseResult.Artists.Contains(author))
						titleParseResult.Artists.Add(author);
				}
			}

			return titleParseResult;
		}

		private Task<VideoUrlParseResult[]> ParsePVs(IDatabaseContext<PVForSong> ctx, string[] urls)
		{
			return Task.WhenAll(urls.Select(url => ParsePV(ctx, url)));
		}

		private async Task<VideoUrlParseResult> ParsePV(IDatabaseContext<PVForSong> ctx, string url)
		{
			if (string.IsNullOrEmpty(url))
				return null;

			var pvResult = await _pvParser.ParseByUrlAsync(url, true, PermissionContext);

			if (!pvResult.IsOk)
				throw pvResult.Exception;

			var existing = ctx.Query().FirstOrDefault(
				s => s.Service == pvResult.Service && s.PVId == pvResult.Id && !s.Song.Deleted);

			if (existing != null)
			{
				throw new VideoParseException($"Song '{existing.Song.TranslatedName[PermissionContext.LanguagePreference]}' already contains this PV");
			}

			return pvResult;
		}

		private ArtistForSong RestoreArtistRef(Song song, Artist artist, ArchivedArtistForSongContract albumRef)
		{
			if (artist != null)
			{
				return (!artist.HasSong(song) ? artist.AddSong(song, albumRef.IsSupport, albumRef.Roles) : null);
			}
			else
			{
				return song.AddArtist(albumRef.NameHint, albumRef.IsSupport, albumRef.Roles);
			}
		}

		public SongQueries(ISongRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IPVParser pvParser, IUserMessageMailer mailer,
			ILanguageDetector languageDetector, IUserIconFactory userIconFactory, IEnumTranslations enumTranslations, IAggregatedEntryImageUrlFactory entryThumbPersister,
			ObjectCache cache, VdbConfigManager config, IEntrySubTypeNameFactory entrySubTypeNameFactory, IFollowedArtistNotifier followedArtistNotifier)
			: base(repository, permissionContext)
		{
			_entryLinkFactory = entryLinkFactory;
			_pvParser = pvParser;
			_mailer = mailer;
			_languageDetector = languageDetector;
			_userIconFactory = userIconFactory;
			_enumTranslations = enumTranslations;
			_entryThumbPersister = entryThumbPersister;
			_cache = cache;
			_config = config;
			_followedArtistNotifier = followedArtistNotifier;
		}

		public ICommentQueries Comments(IDatabaseContext<Song> ctx)
		{
			return new CommentQueries<SongComment, Song>(ctx.OfType<SongComment>(), PermissionContext, _userIconFactory, _entryLinkFactory);
		}

		public ArchivedSongVersion Archive(IDatabaseContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "")
		{
			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedSongVersion>().Save(archived);
			return archived;
		}

		public async Task<ArchivedSongVersion> ArchiveAsync(IDatabaseContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "")
		{
			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			await ctx.SaveAsync(archived);
			return archived;
		}

		public ArchivedSongVersion Archive(IDatabaseContext<Song> ctx, Song song, SongArchiveReason reason, string notes = "")
		{
			return Archive(ctx, song, new SongDiff(), reason, notes);
		}

		public Task<SongContract> Create(CreateSongContract contract)
		{
			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Song needs at least one name", nameof(contract));

			VerifyManageDatabase();

			return _repository.HandleTransactionAsync(async ctx =>
			{
				var pvResults = (await ParsePVs(ctx.OfType<PVForSong>(), contract.PVUrls)).Where(p => p != null).ToArray() ?? new VideoUrlParseResult[0];
				var reprintPvResult = await ParsePV(ctx.OfType<PVForSong>(), contract.ReprintPVUrl);

				ctx.AuditLogger.SysLog($"creating a new song with name '{contract.Names.First().Value}'");

				var diff = new SongDiff();
				diff.Names.Set();
				var song = new Song { SongType = contract.SongType };

				if (contract.OriginalVersion != null && contract.OriginalVersion.Id != 0)
				{
					song.OriginalVersion = ctx.Load(contract.OriginalVersion.Id);
					diff.OriginalVersion.Set();
				}

				song.Names.Init(contract.Names, song);

				await ctx.SaveAsync(song);

				foreach (var artistContract in contract.Artists)
				{
					if (artistContract.Artist != null)
					{
						var artist = await ctx.LoadAsync<Artist>(artistContract.Artist.Id);
						if (!song.HasArtist(artist))
						{
							await ctx.SaveAsync(song.AddArtist(artist, artistContract.IsSupport, artistContract.Roles));
						}
					}
					else
					{
						await ctx.SaveAsync(song.AddArtist(artistContract.Name, artistContract.IsSupport, artistContract.Roles));
					}
				}

				diff.Artists.Set(contract.Artists.Any());

				var pvs = new List<PVContract>();
				Tag[] addedTags = null;

				foreach (var pvResult in pvResults)
				{
					pvs.Add(new PVContract(pvResult, PVType.Original));

					addedTags = AddTagsFromPV(pvResult, song, ctx);
				}

				if (reprintPvResult != null)
				{
					pvs.Add(new PVContract(reprintPvResult, PVType.Reprint));
				}

				var pvDiff = song.SyncPVs(pvs);
				await ctx.SyncAsync(pvDiff);
				diff.PVs.Set(pvs.Any());

				if (contract.WebLinks != null)
				{
					var weblinksDiff = await ctx.SyncAsync(WebLink.Sync(song.WebLinks, contract.WebLinks, song));
					diff.WebLinks.Set(weblinksDiff.Changed);
				}

				if (contract.Lyrics != null && contract.Lyrics.Any())
				{
					contract.Lyrics.ForEach(song.CreateLyrics);
					diff.Lyrics.Set();
				}

				var instrumentalTagId = new EntryTypeTags(ctx).Instrumental;
				song.Status = (contract.Draft || !(new SongValidator().IsValid(song, instrumentalTagId))) ? EntryStatus.Draft : EntryStatus.Finished;

				song.UpdateArtistString();

				var archived = await ArchiveAsync(ctx, song, diff, SongArchiveReason.Created, contract.UpdateNotes ?? string.Empty);
				await ctx.UpdateAsync(song);

				var logStr = $"created song {_entryLinkFactory.CreateEntryLink(song)} of type {song.SongType} ({diff.ChangedFieldsString})"
					+ (!string.IsNullOrEmpty(contract.UpdateNotes) ? " " + HttpUtility.HtmlEncode(contract.UpdateNotes) : string.Empty)
					.Truncate(400);

				await ctx.AuditLogger.AuditLogAsync(logStr);
				await AddEntryEditedEntryAsync(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Created, archived);

				var user = PermissionContext.LoggedUser;

				// Send notifications. Avoid sending notification to the same users twice.
				var notifiedUsers = await _followedArtistNotifier.SendNotificationsAsync(ctx, song, song.ArtistList, user);

				if (addedTags != null && addedTags.Length > 0)
				{
					await new FollowedTagNotifier().SendNotificationsAsync(ctx, song, addedTags, notifiedUsers.Select(u => u.Id).Concat(new[] { user.Id }).ToArray(), _entryLinkFactory, _enumTranslations);
				}

				return new SongContract(song, PermissionContext.LanguagePreference);
			});
		}

		public CommentForApiContract CreateComment(int songId, CommentForApiContract contract)
		{
			return HandleTransaction(ctx => Comments(ctx).Create(songId, contract));
		}

		public (bool created, int reportId) CreateReport(int songId, SongReportType reportType, string hostname, string notes, int? versionNumber)
		{
			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx =>
			{
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					_entryLinkFactory,
					(song, reporter, notesTruncated) => new SongReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != SongReportType.Other ? _enumTranslations.SongReportTypeNames[reportType] : null,
					songId, reportType, hostname, notes);
			});
		}

		public SongForApiContract[] GetByNames(string[] names, SongType[] songTypes, int[] ignoreIds, ContentLanguagePreference lang, int maxResults)
		{
			return HandleQuery(ctx =>
			{
				return ctx.Query<Song>()
					.WhereNotDeleted()
					.Where(s => !ignoreIds.Contains(s.Id))
					.WhereHasName(SearchTextQuery.Create(names, NameMatchMode.StartsWith))
					.WhereHasType(songTypes)
					.Take(maxResults)
					.ToArray()
					.Select(s => new SongForApiContract(s, lang, SongOptionalFields.AdditionalNames))
					.ToArray();
			});
		}

		public CommentForApiContract[] GetComments(int songId)
		{
			return HandleQuery(ctx => Comments(ctx).GetAll(songId));
		}

		public LyricsForSongContract GetLyrics(int lyricsId)
		{
			return HandleQuery(ctx => new LyricsForSongContract(ctx.Load<LyricsForSong>(lyricsId)));
		}

		public SongContract GetSong(int id)
		{
			return HandleQuery(
				session => new SongContract(session.Load<Song>(id), PermissionContext.LanguagePreference));
		}

		public SongDetailsContract GetSongDetails(int songId, int albumId, string hostname, ContentLanguagePreference? languagePreference,
			IEnumerable<OptionalCultureCode> userLanguages)
		{
			return HandleQuery(session =>
			{
				var lang = languagePreference ?? PermissionContext.LanguagePreference;
				var song = session.Load<Song>(songId);
				var contract = new SongDetailsContract(song, lang, GetSongPools(session, songId),
					_config.SpecialTags, new EntryTypeTags(session), PermissionContext, _entryThumbPersister,
					GetSongTypeTag(session, song.SongType)
					);
				var user = PermissionContext.LoggedUser;

				if (user != null)
				{
					var rating = session.Query<FavoriteSongForUser>()
						.FirstOrDefault(s => s.Song.Id == songId && s.User.Id == user.Id);

					contract.UserRating = (rating != null ? rating.Rating : SongVoteRating.Nothing);
				}

				contract.CommentCount = Comments(session).GetCount(songId);
				contract.LatestComments = session.Query<SongComment>()
					.WhereNotDeleted()
					.Where(c => c.EntryForComment.Id == songId)
					.OrderByDescending(c => c.Created).Take(3).ToArray()
					.Select(c => new CommentForApiContract(c, _userIconFactory)).ToArray();
				contract.Hits = session.Query<SongHit>().Count(h => h.Entry.Id == songId);
				contract.ListCount = session.Query<SongInList>().Count(l => l.Song.Id == songId);
				contract.Suggestions = GetSongSuggestions(session, song).Select(s => new SongForApiContract(s, lang, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl)).ToArray();

				contract.PreferredLyrics = LyricsHelper.GetDefaultLyrics(contract.LyricsFromParents, new OptionalCultureCode(CultureInfo.CurrentUICulture, true), userLanguages,
					new Lazy<IEnumerable<UserKnownLanguage>>(() => session.OfType<User>().GetLoggedUserOrNull(_permissionContext)?.KnownLanguages, false));

				if (albumId != 0)
				{
					var album = session.Load<Album>(albumId);

					var track = album.Songs.FirstOrDefault(s => song.Equals(s.Song));

					if (track != null)
					{
						contract.Album = new DataContracts.Albums.AlbumContract(album, lang);

						contract.AlbumSong = new SongInAlbumContract(track, lang, false);

						var previousIndex = album.PreviousTrackIndex(track.Index);
						var previous = album.Songs.FirstOrDefault(s => s.Index == previousIndex);
						contract.PreviousSong = previous != null && previous.Song != null ? new SongInAlbumContract(previous, lang, false) : null;

						var nextIndex = album.NextTrackIndex(track.Index);
						var next = album.Songs.FirstOrDefault(s => s.Index == nextIndex);
						contract.NextSong = next != null && next.Song != null ? new SongInAlbumContract(next, lang, false) : null;
					}
				}

				if (song.Deleted)
				{
					var mergeEntry = GetMergeRecord(session, songId);
					contract.MergedTo = (mergeEntry != null ? new SongContract(mergeEntry.Target, lang) : null);
				}

				AddSongHit(session, song, hostname);

				return contract;
			});
		}

		public SongWithPVAndVoteContract GetSongWithPVAndVote(int songId, bool addHit, string hostname = "", bool includePVs = true)
		{
			return HandleQuery(session =>
			{
				var song = session.Load<Song>(songId);
				FavoriteSongForUser vote = null;

				if (PermissionContext.IsLoggedIn)
				{
					var userId = PermissionContext.LoggedUserId;
					vote = session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == songId && s.User.Id == userId);
				}

				if (addHit)
					AddSongHit(session, song, hostname);

				return new SongWithPVAndVoteContract(song, vote != null ? vote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference, includePVs);
			});
		}

		public SongForApiContract GetSongForApi(int songId, SongOptionalFields fields, ContentLanguagePreference? lang = null)
		{
			return GetSongWithMergeRecord(songId, (song, r) => new SongForApiContract(song, r, lang ?? PermissionContext.LanguagePreference, fields));
		}

		public T GetSong<T>(int id, Func<Song, T> fac)
		{
			return HandleQuery(session =>
			{
				var song = session.Load<Song>(id);
				return fac(song);
			});
		}

		public SongForEditContract GetSongForEdit(int songId)
		{
			return HandleQuery(session => new SongForEditContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));
		}

		public T GetSongWithMergeRecord<T>(int id, Func<Song, SongMergeRecord, T> fac)
		{
			return HandleQuery(session =>
			{
				var song = session.Load<Song>(id);
				return fac(song, (song.Deleted ? GetMergeRecord(session, id) : null));
			});
		}

		public Task<ReadOnlyCollection<TagUsageForApiContract>> GetTagSuggestionsAsync(int songId)
		{
			var maxResults = 3;

			return _repository.HandleQueryAsync(async ctx =>
			{
				var song = await ctx.LoadAsync<Song>(songId);

				var songTags = new HashSet<int>(song.Tags.Tags.Select(t => t.Id));

				var pvResults = await _pvParser.ParseByUrlsAsync(song.PVs
					.Where(pv => pv.PVType == PVType.Original && pv.Service == PVService.NicoNicoDouga)
					.Select(pv => pv.Url), true, _permissionContext);

				var tagMapper = new TagMapper();
				var nicoTags = pvResults.Where(p => p != null).SelectMany(pv => pv.Tags).Distinct().ToArray();
				var entryTypeTags = new EntryTypeTags(ctx);
				var mappedTags = (await MapTagsAsync(ctx, nicoTags)).Where(t => !tagMapper.TagIsRedundantForSong(song.SongType, t.Id, entryTypeTags)).Select(t => t.Id);

				if (song.HasOriginalVersion
					&& song.LengthSeconds > 0
					&& song.OriginalVersion.LengthSeconds > song.LengthSeconds + 30)
				{
					mappedTags = mappedTags.Append(_config.SpecialTags.ShortVersion);
				}

				var instrumentalTagId = entryTypeTags.Instrumental;
				if (song.SongType != SongType.DramaPV
					&& song.SongType != SongType.Instrumental
					&& !ArtistHelper.GetVocalists(song.Artists.ToArray()).Any()
					&& instrumentalTagId != 0)
				{
					mappedTags = mappedTags.Append(instrumentalTagId);
				}

				mappedTags = mappedTags.Where(t => !songTags.Contains(t)).Take(maxResults).ToArray();

				var tags = ctx.LoadMultiple<Tag>(mappedTags);

				var result = await tags.Select(t => new TagUsageForApiContract(t, 0, LanguagePreference)).VdbToListAsync();
				return result.AsReadOnly();
			});
		}

		public async Task<SongForApiContract[]> GetTopRated(
			int? durationHours,
			DateTime? startDate,
			TopSongsDateFilterType? filterBy,
			SongVocalistSelection? vocalist,
			int maxResults,
			SongOptionalFields fields,
			ContentLanguagePreference languagePreference)
		{
			return await _repository.HandleQueryAsync(async ctx =>
			{
				var query = ctx.Query()
					.Where(s => !s.Deleted && s.RatingScore > 0)
					.WhereHasVocalist(vocalist ?? SongVocalistSelection.Nothing);

				if (durationHours.HasValue)
				{
					var timeSpan = TimeSpan.FromHours(durationHours.Value);
					DateTime? endDate = null;

					if (!startDate.HasValue)
					{
						startDate = DateTime.Now - timeSpan;
						endDate = DateTime.Now.AddDays(1);
					}
					else
					{
						endDate = startDate + timeSpan;
					}

					switch (filterBy)
					{
						case TopSongsDateFilterType.PublishDate:
							{
								startDate = startDate?.Date;
								endDate = endDate?.Date;
								query = query.WherePublishDateIsBetween(startDate, endDate);
								break;
							}
						case TopSongsDateFilterType.CreateDate:
							{
								query = query.Where(s => s.CreateDate >= startDate);
								break;
							}
						case TopSongsDateFilterType.Popularity:
							{
								// Sort by number of ratings and hits during that time
								// Older songs get more hits so value them even less
								query = query.OrderByDescending(s => s.UserFavorites
									.Where(f => f.Date >= startDate)
									.Sum(f => (int)f.Rating) + (s.Hits.Count(h => h.Date >= startDate) / 100));
								break;
							}
					}

					if (filterBy != TopSongsDateFilterType.Popularity)
					{
						query = query.OrderByDescending(s => s.RatingScore + (s.Hits.Count / 30));
					}
				}
				else
				{
					query = query.OrderByDescending(s => s.RatingScore);
				}

				var songs = await query
					.Take(maxResults)
					.VdbToListAsync();

				var contracts = songs
					.Select(s => new SongForApiContract(s, null, languagePreference, fields))
					.ToArray();

				return contracts;
			});
		}

		private IEnumerable<(Song song, SongMatchProperty property)> GetNameMatches(IDatabaseContext<Song> ctx, string[] names, int[] artistIds)
		{
			if (names == null || !names.Any())
				return Enumerable.Empty<ValueTuple<Song, SongMatchProperty>>();

			var nameMatchIds = new int[0];

			var totalCount = 10;
			var nameQuery = names.Select(n => SearchTextQuery.Create(n, NameMatchMode.Exact));

			if (artistIds != null && artistIds.Any())
			{
				// Try match with both producer and title
				nameMatchIds = ctx.OfType<Song>().Query()
					.WhereNotDeleted()
					.WhereHasName(nameQuery)
					.Where(n => n.AllArtists.Any(a => a.Artist.ArtistType == ArtistType.Producer && artistIds.Contains(a.Artist.Id)))
					.Select(n => n.Id)
					.OrderBy(s => s)
					.Distinct()
					.Take(totalCount)
					.ToArray();
			}

			if (nameMatchIds.Length < totalCount)
			{
				nameMatchIds = nameMatchIds.Union(ctx.OfType<Song>().Query()
					.WhereNotDeleted()
					.WhereHasName(nameQuery)
					.Select(n => n.Id)
					.OrderBy(s => s)
					.Distinct()
					.Take(totalCount - nameMatchIds.Length)
					.ToArray())
					.ToArray();
			}

			var nameMatches = (nameMatchIds.Any() ? CollectionHelper.SortByIds(ctx.Query().Where(s => nameMatchIds.Contains(s.Id)).ToArray(), nameMatchIds)
				.Select(d => (d, SongMatchProperty.Title)) : new ValueTuple<Song, SongMatchProperty>[] { });

			return nameMatches;
		}

		private (NicoTitleParseResult titleParseResult, string[] names, int[] artistIds) GetPVInfo(IDatabaseContext ctx, VideoUrlParseResult pv, string[] names, int[] artistIds)
		{
			var titleParseResult = ParseNicoPV(ctx.OfType<PVForSong>(), pv);

			if (titleParseResult != null && !string.IsNullOrEmpty(titleParseResult.Title))
				names = names.Concat(new[] { titleParseResult.Title }).ToArray();

			if (titleParseResult != null && titleParseResult.Artists != null && titleParseResult.Artists.Any())
			{
				// Append artists from PV
				var pvArtistIds = titleParseResult.Artists.Select(a => a.Id);
				artistIds = artistIds != null ? artistIds.Union(pvArtistIds).ToArray() : pvArtistIds.ToArray();
			}

			return (titleParseResult, names, artistIds);
		}

		/// <summary>
		/// Parse song and find duplicates in the database.
		/// 
		/// Duplicates are searched based on the entered names and PVs.
		/// Additionally, if PVs are provided, those are parsed for song information such as title and artists.
		/// Titles from the PVs will be used for name matching as well.
		/// </summary>
		/// <param name="anyName">Song names to be searched for duplicates. Cannot be null. Can be empty.</param>
		/// <param name="anyPv">List of PVs to be searched for duplicates and parsed for song info. Cannot be null. Can be empty.</param>
		/// <param name="artistIds">List of artist IDs. Can be empty.</param>
		/// <param name="getPVInfo">
		/// Whether to load song metadata based on the PVs. 
		/// If this is false, only matching for duplicates is done (PV duplicates are still checked).
		/// </param>
		/// <returns>Result of the check. Cannot be null.</returns>
		public async Task<NewSongCheckResultContract> FindDuplicates(string[] anyName, string[] anyPv, int[] artistIds, bool getPVInfo)
		{
			var names = anyName.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToArray();
			var checkedPV = getPVInfo ? anyPv.FirstOrDefault(p => VideoService.NicoNicoDouga.IsValidFor(p)) ?? anyPv.FirstOrDefault() : null; // For downloading video info

			// Parse PV URLs (gets ID and service for each PV). Metadata will be parsed only for the first Nico PV, and only if it's needed.
			var pvs = await Task.WhenAll(anyPv.Select(p => _pvParser.ParseByUrlAsync(p, getPVInfo && p == checkedPV, PermissionContext)));
			pvs = pvs.Where(p => p.IsOk).ToArray();

			if (!names.Any() && !pvs.Any())
				return new NewSongCheckResultContract();

			return HandleQuery(ctx =>
			{
				NicoTitleParseResult titleParseResult = null;
				if (getPVInfo)
				{
					var nicoPV = pvs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga) ?? pvs.FirstOrDefault();
					(titleParseResult, names, artistIds) = GetPVInfo(ctx, nicoPV, names, artistIds);
				}

				var nameMatches = GetNameMatches(ctx, names, artistIds);

				var pvMatches = pvs.Select(pv => ctx.OfType<PVForSong>().Query()
					.Where(p => p.PVId == pv.Id && p.Service == pv.Service)
					.Select(n => n.Song)
					.FirstOrDefault(n => !n.Deleted))
					.Where(p => p != null)
					.Select(d => (song: d, property: SongMatchProperty.PV));


				var matches = pvMatches.Union(nameMatches)
					.Select(s => new DuplicateEntryResultContract<SongMatchProperty>(new EntryRefWithCommonPropertiesContract(s.song, PermissionContext.LanguagePreference), s.property))
					.Distinct(s => s.Entry.Id)
					.ToArray();
				var matches2 = matches;
				return new NewSongCheckResultContract(matches, titleParseResult, PermissionContext.LanguagePreference);
			});
		}

		public RatedSongForUserForApiContract[] GetRatings(int songId, UserOptionalFields userFields, ContentLanguagePreference lang)
		{
			return _repository.HandleQuery(ctx => ctx.Load<Song>(songId).UserFavorites.Select(r =>
			new RatedSongForUserForApiContract(r, _userIconFactory, userFields, PermissionContext)).ToArray());
		}

		public RelatedSongsContract GetRelatedSongs(int songId,
			SongOptionalFields fields,
			ContentLanguagePreference? lang = null)
		{
			var language = lang ?? _permissionContext.LanguagePreference;

			return _repository.HandleQuery(ctx =>
			{
				var song = ctx.Load(songId);
				var songs = new RelatedSongsQuery(ctx).GetRelatedSongs(song);

				return new RelatedSongsContract
				{
					ArtistMatches = songs.ArtistMatches
						.Select(a => new SongForApiContract(a, null, language, fields))
						.OrderBy(a => a.Name)
						.ToArray(),
					LikeMatches = songs.LikeMatches
						.Select(a => new SongForApiContract(a, null, language, fields))
						.OrderBy(a => a.Name)
						.ToArray(),
					TagMatches = songs.TagMatches
						.Select(a => new SongForApiContract(a, null, language, fields))
						.OrderBy(a => a.Name)
						.ToArray()
				};
			});
		}

		public void Merge(int sourceId, int targetId)
		{
			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			_repository.HandleTransaction(ctx =>
			{
				var source = ctx.Load(sourceId);
				var target = ctx.Load(targetId);

				ctx.AuditLogger.AuditLog($"Merging {_entryLinkFactory.CreateEntryLink(source)} to {_entryLinkFactory.CreateEntryLink(target)}");

				// Names
				foreach (var n in source.Names.Names.Where(n => !target.HasName(n)))
				{
					var name = target.CreateName(n.Value, n.Language);
					ctx.Save(name);
				}

				// Weblinks
				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url)))
				{
					var link = target.CreateWebLink(w.Description, w.Url, w.Category, w.Disabled);
					ctx.Save(link);
				}

				// PVs
				var pvs = source.PVs.Where(a => !target.HasPV(a.Service, a.PVId));
				foreach (var p in pvs)
				{
					var pv = target.CreatePV(new PVContract(p));
					ctx.Save(pv);
				}

				// Artist links
				var artists = source.Artists.Where(a => !target.HasArtistLink(a)).ToArray();
				foreach (var a in artists)
				{
					a.Move(target);
					ctx.Update(a);
				}

				// Album links
				var albums = source.Albums.Where(s => !target.IsOnAlbum(s.Album)).ToArray();
				foreach (var s in albums)
				{
					s.Move(target);
					ctx.Update(s);
				}

				// Favorites
				var userFavorites = source.UserFavorites.Where(a => !target.IsFavoritedBy(a.User)).ToArray();
				foreach (var u in userFavorites)
				{
					u.Move(target);
					ctx.Update(u);
				}

				// Custom lists
				var songLists = source.ListLinks.ToArray();
				foreach (var s in songLists)
				{
					s.ChangeSong(target);
					ctx.Update(s);
				}

				// Other properties
				if (target.OriginalVersion == null)
					target.OriginalVersion = source.OriginalVersion;

				var alternateVersions = source.AlternateVersions.ToArray();
				foreach (var alternate in alternateVersions)
				{
					alternate.OriginalVersion = target;
					ctx.Update(alternate);
				}

				if (target.LengthSeconds == 0)
				{
					target.LengthSeconds = source.LengthSeconds;
				}

				target.Notes.CopyIfEmpty(source.Notes);

				if (target.PublishDate.IsEmpty)
				{
					target.PublishDate = source.PublishDate;
				}

				// Create merge record
				var mergeEntry = new SongMergeRecord(source, target);
				ctx.Save(mergeEntry);

				source.Deleted = true;

				target.UpdateArtistString();
				target.UpdateNicoId();

				Archive(ctx, source, SongArchiveReason.Deleted, $"Merged to {target}");
				Archive(ctx, target, SongArchiveReason.Merged, $"Merged from {source}");

				ctx.Update(source);
				ctx.Update(target);
			});
		}

		public PVContract PrimaryPVForSong(int songId, PVServices? pvServices)
		{
			return HandleQuery(ctx =>
			{
				var pvs = ctx.Load(songId).PVs.Where(pv => pvServices == null || (pvServices.Value & (PVServices)pv.Service) == (PVServices)pv.Service);
				var primaryPv = VideoServiceHelper.PrimaryPV(pvs, PermissionContext.IsLoggedIn ? (PVService?)PermissionContext.LoggedUser.PreferredVideoService : null);

				return primaryPv != null ? new PVContract(primaryPv) : null;
			});
		}

		public PVContract PVForSong(int pvId)
		{
			return HandleQuery(session => new PVContract(session.OfType<PVForSong>().Load(pvId)));
		}

		public PVContract PVForSongAndService(int songId, PVService service)
		{
			return HandleQuery(ctx =>
			{
				var pvs = ctx.OfType<PVForSong>().Query().Where(pv => pv.Song.Id == songId && pv.Service == service).ToArray();

				var primaryPv = VideoServiceHelper.PrimaryPV(pvs, service);

				return primaryPv != null ? new PVContract(primaryPv) : null;
			});
		}

		public async Task RefreshPVMetadatas(int songId)
		{
			PermissionContext.VerifyPermission(PermissionToken.AccessManageMenu);

			await _repository.HandleTransactionAsync(async ctx =>
			{
				var song = await ctx.LoadAsync(songId);
				foreach (var pv in song.PVs)
				{
					await pv.RefreshMetadata(_pvParser, PermissionContext);
					await ctx.UpdateAsync(pv);
				}

				song.UpdateLengthFromPV();
				await ctx.UpdateAsync(song);

				ctx.AuditLogger.SysLog("Updated PV metadata for " + song);
			});
		}

		public int RemoveTagUsage(long tagUsageId)
		{
			return new TagUsageQueries(PermissionContext).RemoveTagUsage<SongTagUsage, Song>(tagUsageId, _repository);
		}

		public EntryRevertedContract RevertToVersion(int archivedSongVersionId)
		{
			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session =>
			{
				var archivedVersion = session.Load<ArchivedSongVersion>(archivedSongVersionId);

				if (archivedVersion.Hidden)
				{
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				var song = archivedVersion.Song;

				session.AuditLogger.SysLog("reverting " + song + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedSongContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				song.LengthSeconds = fullProperties.LengthSeconds;
				song.NicoId = fullProperties.NicoId;
				song.Notes.Original = fullProperties.Notes;
				song.Notes.English = fullProperties.NotesEng ?? string.Empty;
				song.PublishDate = fullProperties.PublishDate;
				song.SongType = fullProperties.SongType;
				song.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;
				song.MinMilliBpm = fullProperties.MinMilliBpm;
				song.MaxMilliBpm = fullProperties.MaxMilliBpm;

				// Artists
				var artistDiff = DatabaseContextHelper.RestoreObjectRefs<ArtistForSong, Artist, ArchivedArtistForSongContract>(
					session.OfType<Artist>(), warnings, song.AllArtists, fullProperties.Artists,
					(a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(artist, artistRef) => RestoreArtistRef(song, artist, artistRef),
					artistForSong => artistForSong.Delete());

				if (artistDiff.Changed)
				{
					song.UpdateArtistString();
				}

				// Names
				if (fullProperties.Names != null)
				{
					var nameDiff = song.Names.SyncByContent(fullProperties.Names, song);
					session.Sync(nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null)
				{
					var webLinkDiff = WebLink.SyncByValue(song.WebLinks, fullProperties.WebLinks, song);
					session.Sync(webLinkDiff);
				}

				// Lyrics
				if (fullProperties.Lyrics != null)
				{
					var lyricsDiff = CollectionHelper.Diff(song.Lyrics, fullProperties.Lyrics, (p1, p2) => (p1.Id == p2.Id));

					foreach (var lyrics in lyricsDiff.Added)
					{
						session.Save(song.CreateLyrics(lyrics.Value, lyrics.Source ?? string.Empty, lyrics.URL, lyrics.TranslationType, lyrics.CultureCode));
					}

					foreach (var lyrics in lyricsDiff.Removed)
					{
						song.Lyrics.Remove(lyrics);
						session.Delete(lyrics);
					}

					foreach (var lyrics in lyricsDiff.Unchanged)
					{
						var newLyrics = fullProperties.Lyrics.First(l => l.Id == lyrics.Id);

						lyrics.CultureCode = new OptionalCultureCode(newLyrics.CultureCode);
						lyrics.TranslationType = newLyrics.TranslationType;
						lyrics.Source = newLyrics.Source ?? string.Empty;
						lyrics.Value = newLyrics.Value;
						session.Update(lyrics);
					}
				}

				// PVs
				if (fullProperties.PVs != null)
				{
					var pvDiff = CollectionHelper.Diff(song.PVs, fullProperties.PVs, (p1, p2) => (p1.PVId == p2.PVId && p1.Service == p2.Service));

					foreach (var pv in pvDiff.Added)
					{
						session.Save(song.CreatePV(new PVContract(pv)));
					}

					foreach (var pv in pvDiff.Removed)
					{
						pv.OnDelete();
						session.Delete(pv);
					}
				}

				song.UpdateFavoritedTimes();

				Archive(session, song, SongArchiveReason.Reverted, $"Reverted to version {archivedVersion.Version}");
				AuditLog($"reverted {_entryLinkFactory.CreateEntryLink(song)} to revision {archivedVersion.Version}", session);

				return new EntryRevertedContract(song, warnings);
			});
		}

		public async Task<CollectionDiffWithValue<PVForSong, PVForSong>> UpdatePVs(IDatabaseContext<Song> ctx, Song song, SongDiff diff, IList<PVContract> pvs)
		{
			var oldPublishDate = song.PublishDate;
			var pvDiff = song.SyncPVs(pvs);
			await ctx.OfType<PVForSong>().SyncAsync(pvDiff);

			if (pvDiff.Changed)
			{
				diff.PVs.Set();
				song.UpdatePVServices();
			}

			if (pvDiff.Changed && !oldPublishDate.Equals(song.PublishDate))
			{
				diff.PublishDate.Set();
			}

			return pvDiff;
		}

		public async Task<SongForEditContract> UpdateBasicProperties(SongForEditContract properties)
		{
			ParamIs.NotNull(() => properties);

			VerifyManageDatabase();

			return await _repository.HandleTransactionAsync(async ctx =>
			{
				var song = await ctx.LoadAsync(properties.Id);

				VerifyEntryEdit(song);

				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(PermissionContext)));

				ctx.AuditLogger.SysLog($"updating properties for {song}");

				var oldPvCount = song.PVs.OfType(PVType.Original).Count();
				diff.Notes.Set(song.Notes.CopyFrom(properties.Notes));

				var newOriginalVersion = (properties.OriginalVersion != null && properties.OriginalVersion.Id != 0 ? ctx.Load(properties.OriginalVersion.Id) : null);

				if (!Equals(song.OriginalVersion, newOriginalVersion))
				{
					song.SetOriginalVersion(newOriginalVersion);
					diff.OriginalVersion.Set();
				}

				if (song.SongType != properties.SongType)
				{
					diff.SongType.Set();
					song.SongType = properties.SongType;
				}

				if (song.LengthSeconds != properties.LengthSeconds)
				{
					diff.Length.Set();
					song.LengthSeconds = properties.LengthSeconds;
				}

				if (song.TranslatedName.DefaultLanguage != properties.DefaultNameLanguage)
				{
					song.TranslatedName.DefaultLanguage = properties.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var nameDiff = song.Names.Sync(properties.Names, song);
				await ctx.OfType<SongName>().SyncAsync(nameDiff);

				if (nameDiff.Changed)
					diff.Names.Set();

				var webLinkDiff = WebLink.Sync(song.WebLinks, properties.WebLinks, song);
				await ctx.OfType<SongWebLink>().SyncAsync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks.Set();

				if (song.Status != properties.Status)
				{
					song.Status = properties.Status;
					diff.Status.Set();
				}

				var artistGetter = new Func<ArtistForSongContract, Artist>(artistForSong =>
					ctx.OfType<Artist>().Load(artistForSong.Artist.Id));

				var artistsDiff = song.SyncArtists(properties.Artists, artistGetter);
				ctx.OfType<ArtistForSong>().Sync(artistsDiff);

				if (artistsDiff.Changed)
					diff.Artists.Set();

				var newEvent = new CreateEventQuery().FindOrCreate(ctx, PermissionContext, properties.ReleaseEvent, song);
				if (!song.ReleaseEvent.NullSafeIdEquals(newEvent))
				{
					diff.ReleaseEvent.Set();
					song.SetReleaseEvent(newEvent);
				}

				if (!song.PublishDate.Equals(properties.PublishDate))
				{
					song.PublishDate = properties.PublishDate;
					diff.PublishDate.Set();
				}

				await UpdatePVs(ctx, song, diff, properties.PVs);

				var lyricsDiff = song.SyncLyrics(properties.Lyrics);
				await ctx.OfType<LyricsForSong>().SyncAsync(lyricsDiff);

				if (lyricsDiff.Changed)
					diff.Lyrics.Set();

				var newMinMilliBpm = properties.MinMilliBpm;
				var newMaxMilliBpm = (properties.MaxMilliBpm > properties.MinMilliBpm) ? properties.MaxMilliBpm : properties.MinMilliBpm;
				if (song.MinMilliBpm != newMinMilliBpm || song.MaxMilliBpm != newMaxMilliBpm)
				{
					song.MinMilliBpm = newMinMilliBpm;
					song.MaxMilliBpm = newMaxMilliBpm;
					diff.Bpm.Set();
				}

				var logStr = $"updated properties for song {_entryLinkFactory.CreateEntryLink(song)} ({diff.ChangedFieldsString})"
					+ (properties.UpdateNotes != string.Empty ? " " + HttpUtility.HtmlEncode(properties.UpdateNotes) : string.Empty)
					.Truncate(400);

				var archived = Archive(ctx, song, diff, SongArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				await ctx.UpdateAsync(song);

				await ctx.AuditLogger.AuditLogAsync(logStr);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Updated, archived);

				var newPVCutoff = TimeSpan.FromDays(7);
				if (oldPvCount == 0 && song.PVs.OfType(PVType.Original).Any() && song.CreateDate <= DateTime.Now - newPVCutoff)
				{
					await _followedArtistNotifier.SendNotificationsAsync(ctx, song, song.ArtistList, PermissionContext.LoggedUser);
				}

				var newSongCutoff = TimeSpan.FromHours(1);
				if (artistsDiff.Added.Any() && song.CreateDate >= DateTime.Now - newSongCutoff)
				{
					var addedArtists = artistsDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

					if (addedArtists.Any())
					{
						await _followedArtistNotifier.SendNotificationsAsync(ctx, song, addedArtists, PermissionContext.LoggedUser);
					}
				}

				return new SongForEditContract(song, PermissionContext.LanguagePreference);
			});
		}

		public void UpdatePersonalDescription(int songId, SongDetailsContract data)
		{
			PermissionContext.VerifyLogin();

			HandleTransaction(ctx =>
			{
				var song = ctx.Load(songId);

				EntryPermissionManager.VerifyAccess(PermissionContext, song, EntryPermissionManager.CanEditPersonalDescription);

				song.PersonalDescriptionText = data.PersonalDescriptionText;
				song.PersonalDescriptionAuthorId = data.PersonalDescriptionAuthor?.Id;

				ctx.Update(song);
				ctx.AuditLogger.AuditLog($"updated personal description for {_entryLinkFactory.CreateEntryLink(song)}");
			});
		}

		public void DeleteComment(int commentId) => HandleTransaction(ctx => Comments(ctx).Delete(commentId));

		public IEnumerable<SongForApiContract> GetDerived(int id, SongOptionalFields fields = SongOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => HandleQuery(s => s.Load(id).AlternateVersions.Select(child => new SongForApiContract(child, null, lang, fields)).ToArray());

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

		public void PostEditComment(int commentId, CommentForApiContract contract) => HandleTransaction(ctx => Comments(ctx).Update(commentId, contract));

		public async Task PostPVs(int id, PVContract[] pvs)
		{
			await HandleTransaction(async ctx =>
			{
				var song = await ctx.LoadAsync(id);

				EntryPermissionManager.VerifyEdit(PermissionContext, song);

				var diff = new SongDiff();

				var pvDiff = await UpdatePVs(ctx, song, diff, pvs);

				if (pvDiff.Changed)
				{
					var logStr = $"updated PVs for song {_entryLinkFactory.CreateEntryLink(song)}".Truncate(400);

					await ArchiveAsync(ctx, song, diff, SongArchiveReason.PropertiesUpdated, string.Empty);
					await ctx.UpdateAsync(song);
					await ctx.AuditLogger.AuditLogAsync(logStr);
				}
			});
		}

		public int InstrumentalTagId => HandleQuery(ctx => new EntryTypeTags(ctx).Instrumental);

		public void UpdateArtistString(int id)
		{
			PermissionContext.VerifyPermission(PermissionToken.AccessManageMenu);

			HandleTransaction(ctx =>
			{
				var song = ctx.Load(id);
				song.UpdateArtistString();
				ctx.Update(song);
				ctx.AuditLogger.SysLog("Updated artist string for " + song);
			});
		}

		public void UpdateThumbUrl(int id)
		{
			PermissionContext.VerifyPermission(PermissionToken.AccessManageMenu);

			HandleTransaction(ctx =>
			{
				var song = ctx.Load(id);
				song.UpdateThumbUrl();
				ctx.Update(song);
				ctx.AuditLogger.SysLog("Updated thumbnail URL for " + song);
			});
		}
	}

	public enum TopSongsDateFilterType
	{
		CreateDate,
		PublishDate,
		Popularity
	}
}