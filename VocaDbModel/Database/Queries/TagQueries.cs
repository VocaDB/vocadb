using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository, Tag> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryImagePersisterOld imagePersister;
		private readonly IUserIconFactory userIconFactory;

		private class TagTopUsagesAndCount<T> {

			public T[] TopUsages { get; set; }

			public int TotalCount { get; set; }

		}

		private TagTopUsagesAndCount<TEntry> GetTopUsagesAndCount<TUsage, TEntry, TSort>(
			IDatabaseContext<Tag> ctx, string tagName, 
			Expression<Func<TUsage, bool>> whereExpression, 
			Expression<Func<TUsage, TSort>> createDateExpression,
			Expression<Func<TUsage, TEntry>> selectExpression)
			where TUsage: TagUsage {
			
			var q = TagUsagesQuery<TUsage>(ctx, tagName)
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

		private Tag GetRealTag(IDatabaseContext<Tag> ctx, string tagName, Tag ignoreSelf) {

			if (string.IsNullOrEmpty(tagName))
				return null;

			if (ignoreSelf != null && Tag.Equals(ignoreSelf, tagName))
				return null;

			return ctx.Query().FirstOrDefault(t => t.AliasedTo == null && t.Name == tagName);

		}

		private Tag GetTagById(IDatabaseContext<Tag> ctx, int tagId) {
			return ctx.Query().FirstOrDefault(t => t.Id == tagId);
		}

		private Tag GetTag(IDatabaseContext<Tag> ctx, string name) {

			try {

				var tag = ctx.Load(name);

				if (name != tag.TagName)
					tag = ctx.Load(tag.TagName);

				return tag;

			} catch (ObjectNotFoundException) {
				log.Warn("Tag not found: {0}", name);
				return null;
			}

		}

		// Assumes the tag exists
		private Tag LoadTagById(IDatabaseContext<Tag> ctx, int tagId) {

			var tag = GetTagById(ctx, tagId);

			if (tag == null)
				throw new ObjectNotFoundException(tagId, typeof(Tag));

			return tag;

		}

		private IQueryable<T> TagUsagesQuery<T>(IDatabaseContext<Tag> ctx, string tagName) where T : TagUsage {

			return ctx.OfType<T>().Query().Where(a => a.Tag.Name == tagName);

		}

		public TagQueries(ITagRepository repository, IUserPermissionContext permissionContext,
		                  IEntryLinkFactory entryLinkFactory, IEntryImagePersisterOld imagePersister, IUserIconFactory userIconFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;

		}

		public ArchivedTagVersion Archive(IDatabaseContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedTagVersion>().Save(archived);
			return archived;

		}

		public ICommentQueries Comments(IDatabaseContext<Tag> ctx) {
			return new CommentQueries<TagComment, Tag>(ctx.OfType<TagComment>(), PermissionContext, userIconFactory, entryLinkFactory, tagId => GetTagById(ctx.OfType<Tag>(), tagId));
		}

		public CommentForApiContract CreateComment(int tagId, CommentForApiContract contract) {

			return HandleTransaction(ctx => Comments(ctx).Create(tagId, contract));

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			repository.HandleTransaction(ctx => {

				var tag = LoadTagById(ctx, id);

				tag.Delete();

				var ctxActivity = ctx.OfType<TagActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(t => t.Entry.Id == id).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", tag));

				ctx.Delete(tag);

			});

		}

		public PartialFindResult<T> Find<T>(Func<Tag, T> fac, TagQueryParams queryParams, bool onlyMinimalFields = false)
			where T : class {

			return HandleQuery(ctx => {

				var result = new TagSearch(ctx).Find(queryParams, onlyMinimalFields);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(), result.TotalCount, queryParams.Common.Query, false);

			});

		}

		public PartialFindResult<TagForApiContract> Find(TagQueryParams queryParams, TagOptionalFields optionalFields, bool ssl) {

			return Find(tag => new TagForApiContract(
				tag, imagePersister, ssl, optionalFields), queryParams, optionalFields == TagOptionalFields.None);

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

		public string[] FindNames(TagSearchTextQuery textQuery, string categoryName, TagSortRule sortRule, bool allowAliases, bool allowEmptyName, int maxEntries) {

			if (!allowEmptyName && textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session => {

				var q = session.Query()
					.WhereHasName(textQuery)
					.WhereHasCategoryName(categoryName);

				if (!allowAliases)
					q = q.Where(t => t.AliasedTo == null);

				var tags = q
					.OrderBy(sortRule)
					.Take(maxEntries)
					.Select(t => t.EnglishName)
					.ToArray();

				return tags;

			});

		}

		public CommentForApiContract[] GetComments(int tagId) {

			return HandleQuery(ctx => Comments(ctx).GetAll(tagId));

		}

		public TagDetailsContract GetDetails(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => { 
				
				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;

				var artists = GetTopUsagesAndCount<ArtistTagUsage, Artist, int>(session, tagName, t => !t.Artist.Deleted, t => t.Artist.Id, t => t.Artist);
				var albums = GetTopUsagesAndCount<AlbumTagUsage, Album, int>(session, tagName, t => !t.Album.Deleted, t => t.Album.RatingTotal, t => t.Album);
				var songs = GetTopUsagesAndCount<SongTagUsage, Song, int>(session, tagName, t => !t.Song.Deleted, t => t.Song.RatingScore, t => t.Song);
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
		/// Get tag by name. Returns null if the tag does not exist.
		/// </summary>
		/// <typeparam name="T">Return type.</typeparam>
		/// <param name="tagName">Tag name.</param>
		/// <param name="fac">Return value factory. Cannot be null.</param>
		/// <param name="def">Value to be returned if the tag doesn't exist.</param>
		/// <returns>Return value. This will be <paramref name="def"/> if the tag doesn't exist.</returns>
		public T GetTag<T>(string tagName, Func<Tag, T> fac, T def = default(T)) {
			
			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(ctx => {
				
				var tag = GetTag(ctx, tagName);

				if (tag == null)
					return def;

				return fac(tag);

			});

		}

		public T GetTag<T>(int id, Func<Tag, T> fac) {

			return HandleQuery(ctx => {

				var tag = GetTagById(ctx, id);
				return fac(tag);

			});

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(ctx => {

				var tags = ctx.Query()
					.Where(t => t.AliasedTo == null)
					.OrderBy(t => t.CategoryName)
					.ThenBy(t => t.Name)
					.GroupBy(t => t.CategoryName)
					.ToArray();

				var genres = tags.FirstOrDefault(c => c.Key == Tag.CommonCategory_Genres);
				var empty = tags.FirstOrDefault(c => c.Key == string.Empty);

				var tagsByCategories = 
					new[] { genres }
					.Concat(tags.Except(new[] { genres, empty }))
					.Concat(new[] { empty })
					.Where(t => t != null)
					.Select(t => new TagCategoryContract(t.Key, t))
					.ToArray();

				return tagsByCategories;

			});

		}

		public TagForEditContract GetTagForEdit(int id) {

			return HandleQuery(session => {

				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Id == id && !a.Artist.Deleted) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Id == id && !a.Album.Deleted) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Id == id && !a.Song.Deleted);

				var contract = new TagForEditContract(LoadTagById(session, id), !inUse);

				return contract;

			});

		}

		public int GetTagIdByName(string name) {
			return GetTag(name, t => t.Id);
		}

		public string GetTagNameById(int id) {
			
			return HandleQuery(ctx => {

				var tag = GetTagById(ctx, id);

				if (tag == null)
					throw new ObjectNotFoundException(id, typeof(Tag));

				return tag.Name;

			});

		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(int id) {

			return HandleQuery(ctx => {

				var tag = GetTagById(ctx, id);

				if (tag == null)
					return null;

				return new TagWithArchivedVersionsContract(tag);

			});

		}

		public T LoadTag<T>(int id, Func<Tag, T> fac) {

			return HandleQuery(ctx => fac(LoadTagById(ctx, id)));

		}

		public void Update(TagForEditContract contract, UploadedFileContract uploadedImage) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(contract.Name);

				permissionContext.VerifyEntryEdit(tag);

				var diff = new TagDiff();

				var newAliasedTo = contract.AliasedTo?.Name ?? string.Empty;
				if (!Tag.Equals(tag.AliasedTo, newAliasedTo)) {
					diff.AliasedTo = true;
					tag.AliasedTo = GetRealTag(ctx, newAliasedTo, tag);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName = true;

				if (tag.Description != contract.Description)
					diff.Description = true;

				if (tag.EnglishName != contract.EnglishName)
					diff.Names = true;

				if (!Tag.Equals(tag.Parent, contract.Parent?.Name)) {

					var newParent = GetRealTag(ctx, contract.Parent?.Name, tag);

					if (!Equals(newParent, tag.Parent)) {
						diff.Parent = true;
						tag.SetParent(newParent);						
					}

				}

				if (tag.Status != contract.Status)
					diff.Status = true;

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;
				tag.EnglishName = contract.EnglishName;
				tag.Status = contract.Status;

				if (uploadedImage != null) {

					diff.Picture = true;

					var thumb = new EntryThumb(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, Tag.ImageSizes, originalSize: 500);

				}

				var logStr = string.Format("updated properties for tag {0} ({1})", entryLinkFactory.CreateEntryLink(tag), diff.ChangedFieldsString);
				ctx.AuditLogger.AuditLog(logStr);

				var archived = Archive(ctx, tag, diff, EntryEditEvent.Updated, contract.UpdateNotes);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), tag, EntryEditEvent.Updated, archived);						

				ctx.Update(tag);

			});

		}


	}

}