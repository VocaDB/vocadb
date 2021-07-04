#nullable disable

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Threading.Tasks;
using NHibernate.Exceptions;
using NLog;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
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
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries
{
	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository, Tag>
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly ObjectCache _cache;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IEnumTranslations _enumTranslations;
		private readonly IAggregatedEntryImageUrlFactory _thumbStore;
		private readonly IEntryThumbPersister _imagePersister;
		private readonly IUserIconFactory _userIconFactory;
		private readonly IDiscordWebhookNotifier _discordWebhookNotifier;

		private class TagTopUsagesAndCount<T>
		{
			public IReadOnlyCollection<T> TopUsages { get; set; }

			public int TotalCount { get; set; }
		}

		private async Task<TagTopUsagesAndCount<TEntry>> GetTopUsagesAndCountAsync<TUsage, TEntry, TSort>(
			IDatabaseContext<Tag> ctx, int tagId,
			Expression<Func<TUsage, bool>> whereExpression,
			Expression<Func<TUsage, TSort>> createDateExpression,
			Expression<Func<TUsage, TEntry>> selectExpression,
			int maxCount = 12)
			where TUsage : TagUsage
		{
			var q = TagUsagesQuery<TUsage>(ctx, tagId)
				.Where(whereExpression);

			var topUsages = await q
				.OrderByDescending(t => t.Count)
				.ThenByDescending(createDateExpression)
				.Select(selectExpression)
				.Take(maxCount)
				.VdbToListAsync();

			var usageCount = await q.VdbCountAsync();

			return new TagTopUsagesAndCount<TEntry>
			{
				TopUsages = topUsages,
				TotalCount = usageCount
			};
		}

		private async Task<TagTopUsagesAndCount<TEntry>> GetTopUsagesAndCountAsync<TUsage, TEntry, TSort, TSubType>(
			IDatabaseContext<Tag> ctx, int tagId,
			EntryType entryType,
			Func<IQueryable<TEntry>, EntryTypeAndTagCollection<TSubType>, IQueryable<TEntry>> whereExpression,
			int maxCount = 12)
			where TEntry : class, IEntryBase, IEntryWithTags<TUsage>
			where TUsage : TagUsage
			where TSubType : struct, Enum
		{
			var typeAndTag = EntryTypeAndTagCollection<TSubType>.Create(entryType, tagId, ctx, allowAllTags: true);

			var q = ctx.Query<TEntry>()
				.WhereNotDeleted();

			q = whereExpression(q, typeAndTag);

			IReadOnlyCollection<TEntry> topUsages;
			int usageCount;

			try
			{
				topUsages = await q
					.OrderByTagUsage<TEntry, TUsage>(tagId)
					.Take(maxCount)
					.VdbToListAsync();

				usageCount = await q.VdbCountAsync();
			}
			catch (Exception x) when (x is SqlException or GenericADOException)
			{
				s_log.Warn(x, "Unable to get tag usages");
				topUsages = new TEntry[0];
				usageCount = 0;
			}

			return new TagTopUsagesAndCount<TEntry>
			{
				TopUsages = topUsages,
				TotalCount = usageCount
			};
		}

		private Tag GetRealTag(IDatabaseContext<Tag> ctx, ITag tag, Tag ignoreSelf)
		{
			if (tag == null || tag.Id == 0)
				return null;

			if (ignoreSelf != null && Tag.Equals(ignoreSelf, tag))
				return null;

			return ctx.Get(tag.Id);
		}

		private Tag GetTagByName(IDatabaseContext<Tag> ctx, string name)
		{
			return ctx.Query().WhereHasName(TagSearchTextQuery.Create(name, NameMatchMode.Exact)).FirstOrDefault();
		}

		/// <summary>
		/// Assumes the tag exists - throws an exception if it doesn't.
		/// </summary>
		private Tag LoadTagById(IDatabaseContext<Tag> ctx, int tagId)
		{
			return ctx.Load(tagId);
		}

		private void CreateTrashedEntry(IDatabaseContext<Tag> ctx, Tag tag, string notes)
		{
			var archived = new ArchivedTagContract(tag, new TagDiff(true));
			var data = XmlHelper.SerializeToXml(archived);
			var trashed = new TrashedEntry(tag, data, GetLoggedUser(ctx), notes);

			ctx.Save(trashed);
		}

		private IQueryable<T> TagUsagesQuery<T>(IDatabaseContext<Tag> ctx, int tagId) where T : TagUsage
		{
			return ctx.OfType<T>().Query().Where(a => a.Tag.Id == tagId);
		}

		public TagQueries(
			ITagRepository repository,
			IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			IEntryThumbPersister imagePersister,
			IAggregatedEntryImageUrlFactory thumbStore,
			IUserIconFactory userIconFactory,
			IEnumTranslations enumTranslations,
			ObjectCache cache,
			IDiscordWebhookNotifier discordWebhookNotifier)
			: base(repository, permissionContext)
		{
			_entryLinkFactory = entryLinkFactory;
			_imagePersister = imagePersister;
			_thumbStore = thumbStore;
			_userIconFactory = userIconFactory;
			_enumTranslations = enumTranslations;
			_cache = cache;
			_discordWebhookNotifier = discordWebhookNotifier;
		}

		public ArchivedTagVersion Archive(IDatabaseContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason, string notes = "")
		{
			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedTagVersion.Create(tag, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedTagVersion>().Save(archived);
			return archived;
		}

		public ICommentQueries Comments(IDatabaseContext<Tag> ctx)
		{
			return new CommentQueries<TagComment, Tag>(ctx.OfType<TagComment>(), PermissionContext, _userIconFactory, _entryLinkFactory);
		}

#nullable enable
		/// <summary>
		/// Creates a new tag.
		/// </summary>
		/// <param name="name">Tag English name. Cannot be null or empty. Must be unique.</param>
		/// <returns>The created tag. Cannot be null.</returns>
		/// <exception cref="DuplicateTagNameException">If a tag with the specified name already exists.</exception>
		public async Task<TagBaseContract> Create(string name)
		{
			ParamIs.NotNullOrWhiteSpace(() => name);

			PermissionContext.VerifyManageDatabase();

			return await _repository.HandleTransactionAsync(async ctx =>
			{
				var duplicateName = await ctx.Query<TagName>()
					.Select(t => t.Value)
					.Where(t => t == name)
					.VdbFirstOrDefaultAsync();

				if (duplicateName != null)
				{
					throw new DuplicateTagNameException(duplicateName);
				}

				var factory = new TagFactoryRepository(ctx, ctx.CreateAgentLoginData(PermissionContext));
				var tag = await factory.CreateTagAsync(name);

				await ctx.AuditLogger.AuditLogAsync($"created tag {_entryLinkFactory.CreateEntryLink(tag)}");

				return new TagBaseContract(tag, PermissionContext.LanguagePreference);
			});
		}

		public CommentForApiContract CreateComment(int tagId, CommentForApiContract contract)
		{
			return HandleTransaction(ctx => Comments(ctx).Create(tagId, contract));
		}

		public Task<(bool created, int reportId)> CreateReport(int tagId, TagReportType reportType, string hostname, string notes, int? versionNumber)
		{
			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransactionAsync(ctx =>
			{
				return new Model.Service.Queries.EntryReportQueries().CreateReport(
					ctx,
					PermissionContext,
					_entryLinkFactory,
					(song, reporter, notesTruncated) => new TagReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != TagReportType.Other ? _enumTranslations.Translation(reportType) : null,
					tagId,
					reportType,
					hostname,
					notes,
					_discordWebhookNotifier);
			});
		}
#nullable disable

		public void Delete(int id, string notes)
		{
			PermissionContext.VerifyManageDatabase();

			_repository.HandleTransaction(ctx =>
			{
				var tag = LoadTagById(ctx, id);

				_permissionContext.VerifyEntryDelete(tag);

				tag.Deleted = true;

				ctx.AuditLogger.AuditLog($"deleted {_entryLinkFactory.CreateEntryLink(tag)}");

				Archive(ctx, tag, new TagDiff(false), EntryEditEvent.Deleted, notes);

				ctx.Update(tag);
			});
		}

		private void DeleteActivityEntries(IDatabaseContext<Tag> ctx, int tagId)
		{
			var activityEntries = ctx.Query<TagActivityEntry>().Where(t => t.Entry.Id == tagId).ToArray();
			ctx.DeleteAll(activityEntries);
		}

		private void DeleteReports(IDatabaseContext<Tag> ctx, int tagId)
		{
			var reports = ctx.Query<TagReport>().Where(t => t.Entry.Id == tagId).ToArray();
			ctx.DeleteAll(reports);
		}

		public PartialFindResult<T> Find<T>(Func<Tag, T> fac, TagQueryParams queryParams, bool onlyMinimalFields = false)
			where T : class
		{
			return HandleQuery(ctx =>
			{
				var result = new TagSearch(ctx, queryParams.LanguagePreference).Find(queryParams, onlyMinimalFields);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(), result.TotalCount, queryParams.Common.Query);
			});
		}

		public PartialFindResult<TagForApiContract> Find(TagQueryParams queryParams, TagOptionalFields optionalFields,
			ContentLanguagePreference lang)
		{
			return Find(tag => new TagForApiContract(
				tag, _thumbStore, lang, optionalFields), queryParams, optionalFields == TagOptionalFields.None);
		}

		public string[] FindCategories(SearchTextQuery textQuery)
		{
			return HandleQuery(session =>
			{
				var tags = session.Query()
					.Where(t => t.CategoryName != null && t.CategoryName != "")
					.WhereHasCategoryName(textQuery)
					.Select(t => t.CategoryName)
					.Distinct()
					.ToArray();

				return tags;
			});
		}

		public string[] FindNames(TagSearchTextQuery textQuery, bool allowAliases, int maxEntries)
		{
			return HandleQuery(session =>
			{
				var q = session.Query<TagName>()
					.Where(t => !t.Entry.Deleted)
					.WhereEntryNameIs(textQuery);

				var tags = q
					.Select(t => t.Value)
					.OrderBy(t => t)
					.Take(maxEntries)
					.ToArray();

				return tags;
			});
		}

		public TResult FindTagForEntryType<TResult>(EntryTypeAndSubType entryType, Func<Tag, ContentLanguagePreference, TResult> fac, bool exactOnly = false)
		{
			return HandleQuery(ctx =>
			{
				var tag = ctx.Query<EntryTypeToTagMapping>()
					.Where(m => m.EntryType == entryType.EntryType && m.SubType == entryType.SubType)
					.Select(m => m.Tag)
					.FirstOrDefault();

				if (tag == null && !exactOnly)
				{
					tag = ctx.Query<EntryTypeToTagMapping>()
						.Where(m => m.EntryType == entryType.EntryType && m.SubType == "")
						.Select(m => m.Tag)
						.FirstOrDefault();
				}

				if (tag == null && !exactOnly)
				{
					tag = ctx.Query<EntryTypeToTagMapping>()
						.Where(m => m.EntryType == EntryType.Undefined)
						.Select(m => m.Tag)
						.FirstOrDefault();
				}

				return tag != null ? fac(tag, LanguagePreference) : default;
			});
		}

		public CommentForApiContract[] GetComments(int tagId)
		{
			return HandleQuery(ctx => Comments(ctx).GetAll(tagId));
		}

		private async Task<TagStatsContract> GetStatsAsync(IDatabaseContext<Tag> ctx, int tagId)
		{
			var key = $"TagQueries.GetStats.{tagId}.{LanguagePreference}";
			return await _cache.GetOrInsertAsync(key, CachePolicy.AbsoluteExpiration(1), async () =>
			{
				var artists = await GetTopUsagesAndCountAsync<ArtistTagUsage, Artist, int>(ctx, tagId, t => !t.Entry.Deleted, t => t.Entry.Id, t => t.Entry);
				var albums = await GetTopUsagesAndCountAsync<AlbumTagUsage, Album, int>(ctx, tagId, t => !t.Entry.Deleted, t => t.Entry.RatingTotal, t => t.Entry);
				var songLists = await GetTopUsagesAndCountAsync<SongListTagUsage, SongList, int>(ctx, tagId, t => !t.Entry.Deleted, t => t.Entry.Id, t => t.Entry);
				var songs = await GetTopUsagesAndCountAsync<SongTagUsage, Song, int, SongType>(ctx, tagId, EntryType.Song, (query, etm) => query.WhereHasTypeOrTag(etm));
				var eventSeries = await GetTopUsagesAndCountAsync<EventSeriesTagUsage, ReleaseEventSeries, int>(ctx, tagId, t => !t.Entry.Deleted, t => t.Entry.Id, t => t.Entry, maxCount: 6);
				var seriesIds = eventSeries.TopUsages.Select(e => e.Id).ToArray();

				var eventDateCutoff = DateTime.Now.AddDays(-7);
				var events = await GetTopUsagesAndCountAsync<EventTagUsage, ReleaseEvent, int>(ctx, tagId, t => !t.Entry.Deleted
					&& (t.Entry.Series == null || (t.Entry.Date.DateTime != null && t.Entry.Date.DateTime >= eventDateCutoff) || !seriesIds.Contains(t.Entry.Series.Id)), t => t.Entry.Id, t => t.Entry, maxCount: 6);
				var followerCount = await ctx.Query<TagForUser>().Where(t => t.Tag.Id == tagId).VdbCountAsync();

				var stats = new TagStatsContract(LanguagePreference, _thumbStore,
					artists.TopUsages, artists.TotalCount,
					albums.TopUsages, albums.TotalCount,
					songLists.TopUsages, songLists.TotalCount,
					songs.TopUsages, songs.TotalCount,
					eventSeries.TopUsages, eventSeries.TotalCount,
					events.TopUsages, events.TotalCount,
					followerCount);

				return stats;
			});
		}

		public async Task<TagDetailsContract> GetDetailsAsync(int tagId)
		{
			return await _repository.HandleQueryAsync(async ctx =>
			{
				var tag = await ctx.LoadAsync(tagId);
				var stats = await GetStatsAsync(ctx, tagId);

				var latestComments = await Comments(ctx).GetListAsync(tagId, 3);

				var entryTypeMapping = await ctx.Query<EntryTypeToTagMapping>().Where(etm => etm.Tag == tag).VdbFirstOrDefaultAsync();
				var commentCount = await Comments(ctx).GetCountAsync(tag.Id);
				var isFollowing = _permissionContext.IsLoggedIn && (await ctx.Query<TagForUser>().Where(t => t.Tag.Id == tagId && t.User.Id == _permissionContext.LoggedUserId).VdbAnyAsync());

				return new TagDetailsContract(tag,
					stats,
					LanguagePreference)
				{
					CommentCount = commentCount,
					LatestComments = latestComments,
					IsFollowing = isFollowing,
					RelatedEntryType = entryTypeMapping?.EntryTypeAndSubType ?? new EntryTypeAndSubType()
				};
			});
		}

		public TagEntryMappingContract[] GetEntryMappings()
		{
			return HandleQuery(ctx =>
			{
				return ctx.Query<EntryTypeToTagMapping>()
					.ToArray()
					.OrderBy(t => t.EntryType)
					.ThenBy(t => t.SubType)
					.Select(t => new TagEntryMappingContract(t, LanguagePreference))
					.ToArray();
			});
		}

		public PartialFindResult<TagMappingContract> GetMappings(PagingProperties paging)
		{
			return HandleQuery(ctx =>
			{
				var query = ctx.Query<TagMapping>();

				var result = query
					.OrderByName(LanguagePreference)
					.Paged(paging)
					.ToArray()
					.Select(t => new TagMappingContract(t, LanguagePreference))
					.ToArray();

				return PartialFindResult.Create(result, paging.GetTotalCount ? query.Count() : 0);
			});
		}

#nullable enable
		/// <summary>
		/// Get tag by (exact) name. Returns null if the tag does not exist.
		/// </summary>
		/// <typeparam name="T">Return type.</typeparam>
		/// <param name="tagName">Tag name (exact match, case insensitive).</param>
		/// <param name="fac">Return value factory. Cannot be null.</param>
		/// <param name="def">Value to be returned if the tag doesn't exist.</param>
		/// <returns>Return value. This will be <paramref name="def"/> if the tag doesn't exist.</returns>
		public T? GetTagByName<T>(string tagName, Func<Tag, T> fac, T? def = default(T))
		{
			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(ctx =>
			{
				var tag = GetTagByName(ctx, tagName);

				if (tag == null)
					return def;

				return fac(tag);
			});
		}
#nullable disable

		public TagCategoryContract[] GetTagsByCategories()
		{
			return HandleQuery(ctx =>
			{
				var tags = ctx.Query()
					.Where(t => !t.Deleted)
					.OrderBy(t => t.CategoryName)
					.ThenByEntryName(PermissionContext.LanguagePreference)
					.GroupBy(t => t.CategoryName)
					.ToArray();

				var genres = tags.FirstOrDefault(c => c.Key == TagCommonCategoryNames.Genres);
				var empty = tags.FirstOrDefault(c => c.Key == string.Empty);

				var tagsByCategories =
					new[] { genres }
					.Concat(tags.Except(new[] { genres, empty }))
					.Concat(new[] { empty })
					.Where(t => t != null)
					.Select(t => new TagCategoryContract(t.Key, LanguagePreference, t))
					.ToArray();

				return tagsByCategories;
			});
		}

		/// <summary>
		/// Loads a tag which might not exist.
		/// </summary>
		/// <typeparam name="T">Return type.</typeparam>
		/// <param name="id">Tag Id.</param>
		/// <param name="fac">Return type factory. Cannot be null. This won't be called if the tag is not found.</param>
		/// <param name="def">Value to be returned if the tag is not found.</param>
		/// <returns>
		/// Return value, constructed by <paramref name="fac"/>. If the tag was not found, this will be <paramref name="def"/>.
		/// </returns>
		public T GetTag<T>(int id, Func<Tag, T> fac, T def = default(T))
		{
			return HandleQuery(ctx =>
			{
				var tag = ctx.Get(id);

				if (tag != null)
					return fac(tag);

				return def;
			});
		}

		public TagForEditContract GetTagForEdit(int id)
		{
			return HandleQuery(session =>
			{
				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Id == id && !a.Entry.Deleted) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Id == id && !a.Entry.Deleted) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Id == id && !a.Entry.Deleted);

				var contract = new TagForEditContract(LoadTagById(session, id), !inUse, PermissionContext);

				return contract;
			});
		}

		/// <summary>
		/// Get tag Id by (exact) tag name.
		/// </summary>
		/// <param name="name">Tag name (exact match, case insensitive).</param>
		/// <returns>Tag Id.</returns>
		public int GetTagIdByName(string name)
		{
			return GetTagByName(name, t => t.Id);
		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(int id)
		{
			return LoadTag(id, tag => new TagWithArchivedVersionsContract(tag, LanguagePreference, _userIconFactory));
		}

		public ArchivedTagVersionDetailsContract GetVersionDetails(int id, int comparedVersionId)
		{
			return HandleQuery(session =>
			{
				var contract = new ArchivedTagVersionDetailsContract(session.Load<ArchivedTagVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedTagVersion>(comparedVersionId) : null,
					PermissionContext, _userIconFactory);

				if (contract.Hidden)
				{
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return contract;
			});
		}

		/// <summary>
		/// Loads a tag assuming that the tag exists - throws an exception if it doesn't.
		/// </summary>
		public T LoadTag<T>(int id, Func<Tag, T> fac)
		{
			return HandleQuery(ctx => fac(LoadTagById(ctx, id)));
		}

		public async Task<T> LoadTagAsync<T>(int id, Func<Tag, T> fac)
		{
			return await _repository.HandleQueryAsync(async ctx =>
			{
				var tag = await ctx.LoadAsync(id);
				return fac(tag);
			});
		}

		private void MergeTagUsages(Tag source, Tag target)
		{
			var targetTagUsages = target.AllTagUsages.ToDictionary(t => new GlobalEntryId(t.EntryBase.EntryType, t.EntryBase.Id));

			foreach (var usage in source.AllTagUsages.ToArray())
			{

				if (!targetTagUsages.TryGetValue(new GlobalEntryId(usage.EntryBase.EntryType, usage.EntryBase.Id), out TagUsage targetUsage))
				{
					targetUsage = usage.Move(target);
					source.UsageCount--;
					target.UsageCount++;
				}

				foreach (var vote in usage.VotesBase.Where(v => !targetUsage.HasVoteByUser(v.User)))
				{
					targetUsage.CreateVote(vote.User);
				}
			}
		}

		public void Merge(int sourceId, int targetId)
		{
			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", nameof(targetId));

			_repository.HandleTransaction(ctx =>
			{
				var source = ctx.Load(sourceId);
				var target = ctx.Load(targetId);
				var diff = new TagDiff(false);
				diff.Names.Set();

				ctx.AuditLogger.AuditLog($"Merging {source} to {_entryLinkFactory.CreateEntryLink(target)}");

				// Other properties
				if (string.IsNullOrEmpty(target.CategoryName) && !string.IsNullOrEmpty(source.CategoryName))
				{
					target.CategoryName = source.CategoryName;
					diff.CategoryName.Set();
				}

				diff.Description.Set(target.Description.CopyIfEmpty(source.Description));

				// Parent tag
				if (target.Parent == null && source.Parent != null && target.IsValidParent(source.Parent))
				{
					target.SetParent(source.Parent);
					diff.Parent.Set();
				}

				// Related tags
				foreach (var relatedTag in source.RelatedTags.Select(r => r.LinkedTag)
					.Where(r => !r.Equals(target) && !target.RelatedTags.Any(r2 => r.Equals(r2.LinkedTag))))
				{
					var link = target.AddRelatedTag(relatedTag);
					ctx.Save(link);
					diff.RelatedTags.Set();
				}

				// Weblinks
				foreach (var w in source.WebLinks.Links.Where(w => !target.WebLinks.HasLink(w.Url)))
				{
					var link = target.CreateWebLink(w.Description, w.Url, w.Category, w.Disabled);
					ctx.Save(link);
					diff.WebLinks.Set();
				}

				// Followed tags
				foreach (var user in source.TagsForUsers.Select(r => r.User).Where(u => !target.TagsForUsers.Any(r2 => r2.User.Equals(u))))
				{
					var link = user.AddTag(target);
					ctx.Save(link);
				}

				// Tag mappings
				foreach (var mapping in source.Mappings.Select(r => r.SourceTag).Where(r => target.Mappings.All(r2 => r2.SourceTag != r)))
				{
					var link = target.CreateMapping(mapping);
					ctx.Save(link);
				}

				// Create merge record
				new CreateMergeEntryQuery().CreateMergeEntry<Tag, TagMergeRecord>(ctx, sourceId, target);

				var mergeEntry = new TagMergeRecord(source, target);
				ctx.Save(mergeEntry);

				MergeTagUsages(source, target);

				// Delete entry before copying names
				var names = source.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray();

				CreateTrashedEntry(ctx, source, "Merged to " + target);
				source.Delete();
				DeleteActivityEntries(ctx, sourceId);
				DeleteReports(ctx, sourceId);

				ctx.Delete(source);
				ctx.Flush();

				// Names
				foreach (var n in names)
				{
					var lang = target.Names.HasNameForLanguage(n.Language) ? ContentLanguageSelection.Unspecified : n.Language;
					var name = target.CreateName(n.Value, lang);
					ctx.Save(name);
				}

				Archive(ctx, target, diff, EntryEditEvent.Updated, $"Merged from {source}");

				ctx.Update(target);
			});
		}

		public void MoveToTrash(int id, string notes)
		{
			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			_repository.HandleTransaction(ctx =>
			{
				var tag = LoadTagById(ctx, id);

				_permissionContext.VerifyEntryDelete(tag);

				CreateTrashedEntry(ctx, tag, notes);

				tag.Delete();

				DeleteActivityEntries(ctx, id);
				DeleteReports(ctx, id);

				ctx.AuditLogger.AuditLog($"moved {tag} to trash");

				ctx.Delete(tag);
			});
		}

		public void Restore(int tagId)
		{
			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session =>
			{
				var tag = session.Load<Tag>(tagId);

				tag.Deleted = false;

				Archive(session, tag, new TagDiff(false), EntryEditEvent.Updated);

				AuditLog("restored " + _entryLinkFactory.CreateEntryLink(tag), session);
			});
		}

#nullable enable
		private CollectionDiffWithValue<TagName, TagName> SyncNames(IDatabaseContext<TagName> ctx, Tag tag, LocalizedStringWithIdContract[] names)
		{
			ParamIs.NotNull(() => tag);
			ParamIs.NotNull(() => names);

			var nameValues = names.Select(n => n.Value).ToArray();

			// Verify no duplicates for this tag
			var duplicateName = nameValues
				.GroupBy(n => n, new KanaAndCaseInsensitiveStringComparer())
				.Where(n => n.Count() > 1)
				.Select(n => n.First())
				.FirstOrDefault();

			// Verify no duplicates for other tags
			if (duplicateName == null)
			{
				duplicateName = ctx.Query<TagName>()
					.Where(t => t.Entry.Id != tag.Id && nameValues.Contains(t.Value))
					.Select(t => t.Value)
					.FirstOrDefault();
			}

			if (duplicateName != null)
			{
				throw new DuplicateTagNameException(duplicateName);
			}

			/* 
				We have a unique constraint on tag name.
				By default, NH would first cascade inserts and updates, and only then deletes,
				which would cause a constraint violation. See http://stackoverflow.com/a/707364
				That's why we must manually call Flush after all names have been deleted.
				Updating the names could also cause temporary duplicates, so the names are treated as immutable instead.
			*/
			var diff = tag.Names.Sync(names, tag,
				deleted =>
				{
					foreach (var name in deleted)
						ctx.Delete(name);
					ctx.Flush();
				}, immutable: true
			);

			foreach (var n in diff.Added)
				ctx.Save(n);

			return diff;
		}

		public TagBaseContract Update(TagForEditContract contract, UploadedFileContract? uploadedImage)
		{
			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditTags);

			return _repository.HandleTransaction(ctx =>
			{
				var tag = LoadTagById(ctx, contract.Id);

				_permissionContext.VerifyEntryEdit(tag);

				var diff = new TagDiff();

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName.Set();

				diff.Description.Set(tag.Description.CopyFrom(contract.Description));

				if (tag.HideFromSuggestions != contract.HideFromSuggestions)
					diff.HideFromSuggestions.Set();

				if (tag.Targets != contract.Targets)
					diff.Targets.Set();

				if (tag.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage)
				{
					tag.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var nameDiff = SyncNames(ctx.OfType<TagName>(), tag, contract.Names);

				if (nameDiff.Changed)
				{
					diff.Names.Set();
				}

				if (!Tag.Equals(tag.Parent, contract.Parent))
				{
					var newParent = GetRealTag(ctx, contract.Parent, tag);

					if (!Equals(newParent, tag.Parent))
					{
						diff.Parent.Set();
						tag.SetParent(newParent);
					}
				}

				var relatedTagsDiff = tag.SyncRelatedTags(contract.RelatedTags, tagId => ctx.Load(tagId));
				ctx.Sync(relatedTagsDiff);
				diff.RelatedTags.Set(relatedTagsDiff.Changed);

				var webLinkDiff = tag.WebLinks.Sync(contract.WebLinks, tag);
				ctx.OfType<TagWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks.Set();

				if (tag.Status != contract.Status)
					diff.Status.Set();

				tag.CategoryName = contract.CategoryName;
				tag.HideFromSuggestions = contract.HideFromSuggestions;
				tag.Status = contract.Status;
				tag.Targets = contract.Targets;

				if (uploadedImage != null)
				{
					diff.Picture.Set();

					var thumb = new EntryThumbMain(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(_imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, Tag.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);
				}

				var logStr = $"updated properties for tag {_entryLinkFactory.CreateEntryLink(tag)} ({diff.ChangedFieldsString})";
				ctx.AuditLogger.AuditLog(logStr);

				var archived = Archive(ctx, tag, diff, EntryEditEvent.Updated, contract.UpdateNotes);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), tag, EntryEditEvent.Updated, archived);

				ctx.Update(tag);

				return new TagBaseContract(tag, LanguagePreference);
			});
		}
#nullable disable

		public void UpdateEntryMappings(TagEntryMappingContract[] mappings)
		{
			PermissionContext.VerifyPermission(PermissionToken.ManageTagMappings);

			HandleTransaction(ctx =>
			{
				ctx.AuditLogger.SysLog("updating entry type / tag mappings");

				var existing = ctx.Query<EntryTypeToTagMapping>().ToList();
				var diff = CollectionHelper.Sync(existing, mappings, (m, m2) => m.EntryTypeAndSubType.Equals(m2.EntryType) && m.Tag.IdEquals(m2.Tag),
					create: t => new EntryTypeToTagMapping(t.EntryType, ctx.Load<Tag>(t.Tag.Id)));

				ctx.Sync(diff);

				var diffMessage = $"added [{string.Join(", ", diff.Added.Select(t => t.Tag.DefaultName))}], deleted [{string.Join(", ", diff.Removed.Select(t => t.Tag.DefaultName))}]";
				ctx.AuditLogger.AuditLog($"updated entry type / tag mappings ({diffMessage})");
				ctx.AuditLogger.SysLog(diffMessage);
			});
		}

		public void UpdateMappings(TagMappingContract[] mappings)
		{
			TagMapping CreateMapping(IDatabaseContext<Tag> ctx, TagMappingContract contract)
			{
				return ctx.LoadEntry<Tag>(contract.Tag).CreateMapping(contract.SourceTag);
			}

			PermissionContext.VerifyPermission(PermissionToken.ManageTagMappings);

			HandleTransaction(ctx =>
			{
				ctx.AuditLogger.SysLog("updating tag mappings");

				var existing = ctx.Query<TagMapping>().ToList();
				var diff = CollectionHelper.Sync(existing, mappings, (m, m2) => m.SourceTag == m2.SourceTag && m.Tag.IdEquals(m2.Tag),
					create: t => CreateMapping(ctx, t), remove: mapping => mapping.Delete());

				ctx.Sync(diff);

				var diffMessage = $"added [{string.Join(", ", diff.Added.Select(t => t.Tag.DefaultName))}], deleted [{string.Join(", ", diff.Removed.Select(t => t.Tag.DefaultName))}]";
				ctx.AuditLogger.AuditLog($"updated tag mappings ({diffMessage})");
				ctx.AuditLogger.SysLog(diffMessage);
			});
		}

		public void DeleteComment(int commentId) => HandleTransaction(ctx => Comments(ctx).Delete(commentId));

		public TagForApiContract[] GetChildTags(int tagId,
			TagOptionalFields fields = TagOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => HandleQuery(ctx => ctx.Load(tagId).Children.Select(t => new TagForApiContract(t, _thumbStore, lang, fields)).ToArray());

		public TagBaseContract[] GetTopTags(string categoryName = null, EntryType? entryType = null,
			int maxResults = 15,
			ContentLanguagePreference lang = ContentLanguagePreference.Default)
		{
			return
				HandleQuery(ctx =>
				{
					return ctx.Query<Tag>()
						.WhereHasCategoryName(categoryName)
						.OrderByUsageCount(entryType)
						.Paged(new PagingProperties(0, maxResults, false))
						.Select(t => new TagBaseContract(t, lang, false, false))
						.ToArray();
				})
				.OrderBy(t => t.Name)
				.ToArray();
		}

		public void PostEditComment(int commentId, CommentForApiContract contract) => HandleTransaction(ctx => Comments(ctx).Update(commentId, contract));

		public int InstrumentalTagId => HandleQuery(ctx => new EntryTypeTags(ctx).Instrumental);
	}
}
