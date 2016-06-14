using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository, Tag> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEnumTranslations enumTranslations;
		private readonly IEntryImagePersisterOld imagePersister;
		private readonly IUserIconFactory userIconFactory;

		private class TagTopUsagesAndCount<T> {

			public T[] TopUsages { get; set; }

			public int TotalCount { get; set; }

		}

		private TagTopUsagesAndCount<TEntry> GetTopUsagesAndCount<TUsage, TEntry, TSort>(
			IDatabaseContext<Tag> ctx, int tagId, 
			Expression<Func<TUsage, bool>> whereExpression, 
			Expression<Func<TUsage, TSort>> createDateExpression,
			Expression<Func<TUsage, TEntry>> selectExpression)
			where TUsage: TagUsage {
			
			var q = TagUsagesQuery<TUsage>(ctx, tagId)
				.Where(whereExpression);

			var topUsages = q
				.OrderByDescending(t => t.Count)
				.ThenByDescending(createDateExpression)
				.Select(selectExpression)
				.Take(12)
				.ToArray();

			var usageCount = q.Count();

			return new TagTopUsagesAndCount<TEntry> {
				TopUsages = topUsages, TotalCount = usageCount
			};

		}

		private Tag GetRealTag(IDatabaseContext<Tag> ctx, ITag tag, Tag ignoreSelf) {

			if (tag == null || tag.Id == 0)
				return null;

			if (ignoreSelf != null && Tag.Equals(ignoreSelf, tag))
				return null;

			return ctx.Query().FirstOrDefault(t => t.AliasedTo == null && t.Id == tag.Id);

		}

		private Tag GetTagByName(IDatabaseContext<Tag> ctx, string name) {

			var tag = ctx.Query().WhereHasName(TagSearchTextQuery.Create(name, NameMatchMode.Exact)).FirstOrDefault();

			if (tag == null) {
				log.Warn("Tag not found: {0}", name);
			}

			return tag;

		}

		/// <summary>
		/// Assumes the tag exists - throws an exception if it doesn't.
		/// </summary>
		private Tag LoadTagById(IDatabaseContext<Tag> ctx, int tagId) {

			return ctx.Load(tagId);

		}

		private void CreateTrashedEntry(IDatabaseContext<Tag> ctx, Tag tag, string notes) {

			var archived = new ArchivedTagContract(tag, new TagDiff(true));
			var data = XmlHelper.SerializeToXml(archived);
			var trashed = new TrashedEntry(tag, data, GetLoggedUser(ctx), notes);

			ctx.Save(trashed);

		}

		private IQueryable<T> TagUsagesQuery<T>(IDatabaseContext<Tag> ctx, int tagId) where T : TagUsage {

			return ctx.OfType<T>().Query().Where(a => a.Tag.Id == tagId);

		}

		public TagQueries(ITagRepository repository, IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory, IEntryImagePersisterOld imagePersister, IUserIconFactory userIconFactory,
			IEnumTranslations enumTranslations)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;
			this.enumTranslations = enumTranslations;

		}

		public ArchivedTagVersion Archive(IDatabaseContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedTagVersion.Create(tag, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedTagVersion>().Save(archived);
			return archived;

		}

		public ICommentQueries Comments(IDatabaseContext<Tag> ctx) {
			return new CommentQueries<TagComment, Tag>(ctx.OfType<TagComment>(), PermissionContext, userIconFactory, entryLinkFactory);
		}

		/// <summary>
		/// Creates a new tag.
		/// </summary>
		/// <param name="name">Tag English name. Cannot be null or empty. Must be unique.</param>
		/// <returns>The created tag. Cannot be null.</returns>
		/// <exception cref="DuplicateTagNameException">If a tag with the specified name already exists.</exception>
		public TagBaseContract Create(string name) {

			ParamIs.NotNullOrWhiteSpace(() => name);

			PermissionContext.VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				var duplicateName = ctx.Query<TagName>()
					.Select(t => t.Value)
					.FirstOrDefault(t => t == name);

				if (duplicateName != null) {
					throw new DuplicateTagNameException(duplicateName);
				}

				var factory = new TagFactoryRepository(ctx, ctx.CreateAgentLoginData(PermissionContext));
				var tag = factory.CreateTag(name);

				ctx.AuditLogger.AuditLog(string.Format("created {0}", tag));

				return new TagBaseContract(tag, PermissionContext.LanguagePreference);

			});

		}

		public CommentForApiContract CreateComment(int tagId, CommentForApiContract contract) {

			return HandleTransaction(ctx => Comments(ctx).Create(tagId, contract));

		}

		public bool CreateReport(int tagId, TagReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory, report => report.Entry.Id == tagId,
					(song, reporter, notesTruncated) => new TagReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != TagReportType.Other ? enumTranslations.Translation(reportType) : null,
					tagId, reportType, hostname, notes);
			});

		}

		public void Delete(int id, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			repository.HandleTransaction(ctx => {

				var tag = LoadTagById(ctx, id);

				permissionContext.VerifyEntryDelete(tag);

				tag.Deleted = true;

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", tag));

				Archive(ctx, tag, new TagDiff(false), EntryEditEvent.Deleted, notes);

				ctx.Update(tag);

			});

		}

		private void DeleteActivityEntries(IDatabaseContext<Tag> ctx, int tagId) {

			var ctxActivity = ctx.OfType<TagActivityEntry>();
			var activityEntries = ctxActivity.Query().Where(t => t.Entry.Id == tagId).ToArray();

			foreach (var activityEntry in activityEntries)
				ctxActivity.Delete(activityEntry);

		}

		private void DeleteReports(IDatabaseContext<Tag> ctx, int tagId) {

			var reports = ctx.Query<TagReport>().Where(t => t.Entry.Id == tagId).ToArray();
			ctx.DeleteAll(reports);

		}

		public PartialFindResult<T> Find<T>(Func<Tag, T> fac, TagQueryParams queryParams, bool onlyMinimalFields = false)
			where T : class {

			return HandleQuery(ctx => {

				var result = new TagSearch(ctx, LanguagePreference).Find(queryParams, onlyMinimalFields);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(), result.TotalCount, queryParams.Common.Query);

			});

		}

		public PartialFindResult<TagForApiContract> Find(TagQueryParams queryParams, TagOptionalFields optionalFields, bool ssl,
			ContentLanguagePreference lang) {

			return Find(tag => new TagForApiContract(
				tag, imagePersister, ssl, lang, optionalFields), queryParams, optionalFields == TagOptionalFields.None);

		}

		public string[] FindCategories(SearchTextQuery textQuery) {

			return HandleQuery(session => {

				var tags = session.Query()
					.Where(t => t.CategoryName != null && t.CategoryName != "")
					.WhereHasCategoryName(textQuery)
					.Select(t => t.CategoryName)
					.Distinct()
					.ToArray();

				return tags;

			});

		}

		public string[] FindNames(TagSearchTextQuery textQuery, bool allowAliases, int maxEntries) {

			return HandleQuery(session => {

				var q = session.Query<TagName>()
					.Where(t => !t.Entry.Deleted)
					.WhereEntryNameIs(textQuery);

				if (!allowAliases)
					q = q.Where(t => t.Entry.AliasedTo == null);

				var tags = q
					.Select(t => t.Value)
					.OrderBy(t => t)
					.Take(maxEntries)
					.ToArray();

				return tags;

			});

		}

		public CommentForApiContract[] GetComments(int tagId) {

			return HandleQuery(ctx => Comments(ctx).GetAll(tagId));

		}

		public TagDetailsContract GetDetails(int tagId) {

			return HandleQuery(session => {

				var tag = LoadTagById(session, tagId);

				var artists = GetTopUsagesAndCount<ArtistTagUsage, Artist, int>(session, tagId, t => !t.Artist.Deleted, t => t.Artist.Id, t => t.Artist);
				var albums = GetTopUsagesAndCount<AlbumTagUsage, Album, int>(session, tagId, t => !t.Album.Deleted, t => t.Album.RatingTotal, t => t.Album);
				var songs = GetTopUsagesAndCount<SongTagUsage, Song, int>(session, tagId, t => !t.Song.Deleted, t => t.Song.RatingScore, t => t.Song);
				var latestComments = Comments(session).GetList(tag.Id, 3);

				return new TagDetailsContract(tag,
					artists.TopUsages, artists.TotalCount,
					albums.TopUsages, albums.TotalCount,
					songs.TopUsages, songs.TotalCount,
					PermissionContext.LanguagePreference) {
					CommentCount = Comments(session).GetCount(tag.Id),
					LatestComments = latestComments
				};
				
			});

		}

		/// <summary>
		/// Get tag by (exact) name. Returns null if the tag does not exist.
		/// </summary>
		/// <typeparam name="T">Return type.</typeparam>
		/// <param name="tagName">Tag name (exact match, case insensitive).</param>
		/// <param name="fac">Return value factory. Cannot be null.</param>
		/// <param name="def">Value to be returned if the tag doesn't exist.</param>
		/// <returns>Return value. This will be <paramref name="def"/> if the tag doesn't exist.</returns>
		public T GetTagByName<T>(string tagName, Func<Tag, T> fac, T def = default(T)) {
			
			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(ctx => {
				
				var tag = GetTagByName(ctx, tagName);

				if (tag == null)
					return def;

				return fac(tag);

			});

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(ctx => {

				var tags = ctx.Query()
					.Where(t => t.AliasedTo == null && !t.Deleted)
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
		public T GetTag<T>(int id, Func<Tag, T> fac, T def = default(T)) {

			return HandleQuery(ctx => {

				var tag = ctx.Get(id);

				if (tag != null)
					return fac(tag);

				return def;

			});

		}

		public TagForEditContract GetTagForEdit(int id) {

			return HandleQuery(session => {

				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Id == id && !a.Artist.Deleted) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Id == id && !a.Album.Deleted) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Id == id && !a.Song.Deleted);

				var contract = new TagForEditContract(LoadTagById(session, id), !inUse, PermissionContext);

				return contract;

			});

		}

		/// <summary>
		/// Get tag Id by (exact) tag name.
		/// </summary>
		/// <param name="name">Tag name (exact match, case insensitive).</param>
		/// <returns>Tag Id.</returns>
		public int GetTagIdByName(string name) {
			return GetTagByName(name, t => t.Id);
		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(int id) {

			return LoadTag(id, tag => new TagWithArchivedVersionsContract(tag, LanguagePreference));

		}

		public ArchivedTagVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedTagVersionDetailsContract(session.Load<ArchivedTagVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedTagVersion>(comparedVersionId) : null,
					PermissionContext.LanguagePreference));

		}

		/// <summary>
		/// Loads a tag assuming that the tag exists - throws an exception if it doesn't.
		/// </summary>
		public T LoadTag<T>(int id, Func<Tag, T> fac) {

			return HandleQuery(ctx => fac(LoadTagById(ctx, id)));

		}

		private void MergeTagUsages(Tag source, Tag target) {

			var targetTagUsages = target.AllTagUsages.ToDictionary(t => new GlobalEntryId(t.Entry.EntryType, t.Entry.Id));

			foreach (var usage in source.AllTagUsages.ToArray()) {

				TagUsage targetUsage;

				if (!targetTagUsages.TryGetValue(new GlobalEntryId(usage.Entry.EntryType, usage.Entry.Id), out targetUsage)) {
					targetUsage = usage.Move(target);
					source.UsageCount--;
					target.UsageCount++;
				}

				foreach (var vote in usage.VotesBase.Where(v => !targetUsage.HasVoteByUser(v.User))) {
					targetUsage.CreateVote(vote.User);
				}

			}

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", nameof(targetId));

			repository.HandleTransaction(ctx => {

				var source = ctx.Load(sourceId);
				var target = ctx.Load(targetId);
				var diff = new TagDiff(false);
				diff.Names.Set();

				ctx.AuditLogger.AuditLog(string.Format("Merging {0} to {1}",
					source, entryLinkFactory.CreateEntryLink(target)));

				// Other properties
				if (string.IsNullOrEmpty(target.CategoryName) && !string.IsNullOrEmpty(source.CategoryName)) {
					target.CategoryName = source.CategoryName;
					diff.CategoryName.Set();
				}

				diff.Description.Set(target.Description.CopyIfEmpty(source.Description));

				// Parent tag
				if (target.Parent == null && source.Parent != null) {
					target.SetParent(source.Parent);
					diff.Parent.Set();
				}

				// Related tags
				foreach (var relatedTag in source.RelatedTags.Select(r => r.LinkedTag)
					.Where(r => !r.Equals(target) && !target.RelatedTags.Any(r2 => r.Equals(r2.LinkedTag)))) {
					var link = target.AddRelatedTag(relatedTag);
					ctx.Save(link);
					diff.RelatedTags.Set();
				}

				// Weblinks
				foreach (var w in source.WebLinks.Links.Where(w => !target.WebLinks.HasLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					ctx.Save(link);
					diff.WebLinks.Set();
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

				ctx.Delete(source);
				ctx.Flush();

				// Names
				foreach (var n in names) {
					var lang = target.Names.HasNameForLanguage(n.Language) ? ContentLanguageSelection.Unspecified : n.Language;
					var name = target.CreateName(n.Value, lang);
					ctx.Save(name);
				}

				Archive(ctx, target, diff, EntryEditEvent.Updated, string.Format("Merged from {0}", source));

				ctx.Update(target);

			});

		}

		public void MoveToTrash(int id, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			repository.HandleTransaction(ctx => {

				var tag = LoadTagById(ctx, id);

				permissionContext.VerifyEntryDelete(tag);

				CreateTrashedEntry(ctx, tag, notes);

				tag.Delete();

				DeleteActivityEntries(ctx, id);
				DeleteReports(ctx, id);

				ctx.AuditLogger.AuditLog(string.Format("moved {0} to trash", tag));

				ctx.Delete(tag);

			});

		}

		public void Restore(int songId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var tag = session.Load<Tag>(songId);

				tag.Deleted = false;

				Archive(session, tag, new TagDiff(false), EntryEditEvent.Updated);

				AuditLog("restored " + entryLinkFactory.CreateEntryLink(tag), session);

			});

		}

		private CollectionDiffWithValue<TagName, TagName> SyncNames(IDatabaseContext<TagName> ctx, Tag tag, LocalizedStringWithIdContract[] names) {

			ParamIs.NotNull(() => tag);
			ParamIs.NotNull(() => names);

			var nameValues = names.Select(n => n.Value).ToArray();

			// Verify no duplicates for this tag
			var duplicateName = nameValues
				.GroupBy(n => n)
				.Where(n => n.Count() > 1)
				.Select(n => n.First())
				.FirstOrDefault();

			// Verify no duplicates for other tags
			if (duplicateName == null) {
				duplicateName = ctx.Query<TagName>()
					.Where(t => t.Entry.Id != tag.Id && nameValues.Contains(t.Value))
					.Select(t => t.Value)
					.FirstOrDefault();
			}

			if (duplicateName != null) {
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
				deleted => {
					foreach (var name in deleted)
						ctx.Delete(name);
					ctx.Flush();
				}, immutable: true
			);
	
			foreach (var n in diff.Added)
				ctx.Save(n);

			return diff;

		}

		public TagBaseContract Update(TagForEditContract contract, UploadedFileContract uploadedImage) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			return repository.HandleTransaction(ctx => {

				var tag = LoadTagById(ctx, contract.Id);

				permissionContext.VerifyEntryEdit(tag);

				var diff = new TagDiff();

				if (!Tag.Equals(tag.AliasedTo, contract.AliasedTo)) {
					diff.AliasedTo.Set();
					tag.AliasedTo = GetRealTag(ctx, contract.AliasedTo, tag);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName.Set();

				diff.Description.Set(tag.Description.CopyFrom(contract.Description));

				if (tag.HideFromSuggestions != contract.HideFromSuggestions)
					diff.HideFromSuggestions.Set();

				if (tag.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage) {
					tag.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var nameDiff = SyncNames(ctx.OfType<TagName>(), tag, contract.Names);

				if (nameDiff.Changed) {					
					diff.Names.Set();
				}

				if (!Tag.Equals(tag.Parent, contract.Parent)) {

					var newParent = GetRealTag(ctx, contract.Parent, tag);

					if (!Equals(newParent, tag.Parent)) {
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

				if (uploadedImage != null) {

					diff.Picture.Set();

					var thumb = new EntryThumb(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, Tag.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);

				}

				var logStr = string.Format("updated properties for tag {0} ({1})", entryLinkFactory.CreateEntryLink(tag), diff.ChangedFieldsString);
				ctx.AuditLogger.AuditLog(logStr);

				var archived = Archive(ctx, tag, diff, EntryEditEvent.Updated, contract.UpdateNotes);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), tag, EntryEditEvent.Updated, archived);						

				ctx.Update(tag);

				return new TagBaseContract(tag, LanguagePreference);

			});

		}


	}

}