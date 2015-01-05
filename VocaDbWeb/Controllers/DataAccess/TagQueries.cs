using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries for <see cref="Tag"/>.
	/// </summary>
	public class TagQueries : QueriesBase<ITagRepository, Tag> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryImagePersisterOld imagePersister;

		private class TagTopUsagesAndCount<T> {

			public T[] TopUsages { get; set; }

			public int TotalCount { get; set; }

		}

		private TagTopUsagesAndCount<TEntry> GetTopUsagesAndCount<TUsage, TEntry, TSort>(
			IRepositoryContext<Tag> ctx, string tagName, 
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

		private Tag GetRealTag(IRepositoryContext<Tag> ctx, string tagName, Tag ignoreSelf) {

			if (string.IsNullOrEmpty(tagName))
				return null;

			if (ignoreSelf != null && Tag.Equals(ignoreSelf, tagName))
				return null;

			return ctx.Query().FirstOrDefault(t => t.AliasedTo == null && t.Name == tagName);

		}

		private Tag GetTag(IRepositoryContext<Tag> ctx, string name) {

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

		private IQueryable<T> TagUsagesQuery<T>(IRepositoryContext<Tag> ctx, string tagName) where T : TagUsage {

			return ctx.OfType<T>().Query().Where(a => a.Tag.Name == tagName);

		}

		private int TagUsagesCount<T, TEntry>(IRepositoryContext<Tag> ctx, string[] tagNames, Expression<Func<T, TEntry>> entryFunc)
			where T : TagUsage
			where TEntry : IEntryBase {

			return ctx.OfType<T>().Query().Where(a => tagNames.Contains(a.Tag.Name)).Select(entryFunc).Where(e => !e.Deleted).Distinct().Count();

		}

		private IEnumerable<TEntry> TagUsagesQuery<T, TEntry>(IRepositoryContext<Tag> ctx, string[] tagNames, int count, Expression<Func<T, TEntry>> entryFunc) 
			where T : TagUsage where TEntry : IEntryBase {

			return ctx.OfType<T>().Query()
				.Where(a => tagNames.Contains(a.Tag.Name))
				.OrderByDescending(u => u.Count)
				.Select(entryFunc)
				.Where(e => !e.Deleted)
				.Take(count)
				.ToArray()
				.Distinct();

		}

		public TagQueries(ITagRepository repository, IUserPermissionContext permissionContext,
		                  IEntryLinkFactory entryLinkFactory, IEntryImagePersisterOld imagePersister)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;

		}

		public void Archive(IRepositoryContext<Tag> ctx, Tag tag, TagDiff diff, EntryEditEvent reason) {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason);
			ctx.OfType<ArchivedTagVersion>().Save(archived);

		}

		public void Delete(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(name);

				tag.Delete();

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", tag));

				ctx.Delete(tag);

			});

		}

		public PartialFindResult<T> Find<T>(Func<Tag, T> fac, CommonSearchParams queryParams, PagingProperties paging, 
			bool allowAliases = false, string categoryName = "")
			where T : class {

			var textQuery = TagSearchTextQuery.Create(queryParams.Query, queryParams.NameMatchMode);

			return HandleQuery(ctx => {

				var query = ctx.Query()
					.WhereHasName(textQuery)
					.WhereAllowAliases(allowAliases)
					.WhereHasCategoryName(categoryName);

				var tags = query
					.OrderBy(t => t.Name)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = 0;

				if (paging.GetTotalCount) {
					
					count = query.Count();

				}

				var result = tags.Select(fac).ToArray();

				return new PartialFindResult<T>(result, count, queryParams.Query, false);

			});

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

		public string[] FindNames(TagSearchTextQuery textQuery, bool allowAliases, bool allowEmptyName, int maxEntries) {

			if (!allowEmptyName && textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session => {

				var q = session.Query()
					.WhereHasName(textQuery);

				if (!allowAliases)
					q = q.Where(t => t.AliasedTo == null);

				var tags = q
					.OrderBy(t => t.Name)
					.Take(maxEntries)
					.Select(t => t.Name)
					.ToArray();

				return tags;

			});

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

				return new TagDetailsContract(tag, 
					artists.TopUsages, artists.TotalCount, 
					albums.TopUsages, albums.TotalCount, 
					songs.TopUsages, songs.TotalCount, 
					PermissionContext.LanguagePreference);
				
			});

		}

		public T GetTag<T>(string tagName, Func<Tag, T> fac) {
			
			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(ctx => {
				
				var tag = GetTag(ctx, tagName);
				return fac(tag);

			});

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(ctx => {

				var tags = ctx.Query()
					.Where(t => t.AliasedTo == null)
					.OrderBy(t => t.Name)
					.ToArray()					
					.GroupBy(t => t.CategoryName)
					.ToArray();

				var empty = tags.Where(c => c.Key == string.Empty).ToArray();

				var tagsByCategories = tags
					.Except(empty).Concat(empty)
					.Select(t => new TagCategoryContract(t.Key, t))
					.ToArray();

				return tagsByCategories;

			});

		}

		public TagForEditContract GetTagForEdit(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => {

				var inUse = session.Query<ArtistTagUsage>().Any(a => a.Tag.Name == tagName && !a.Artist.Deleted) ||
					session.Query<AlbumTagUsage>().Any(a => a.Tag.Name == tagName && !a.Album.Deleted) ||
					session.Query<SongTagUsage>().Any(a => a.Tag.Name == tagName && !a.Song.Deleted);

				var contract = new TagForEditContract(session.Load(tagName), !inUse);

				return contract;

			});

		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(string tagName) {

			return HandleQuery(ctx => {

				var tag = GetTag(ctx, tagName);

				if (tag == null)
					return null;

				return new TagWithArchivedVersionsContract(tag);

			});

		}


		public void Update(TagContract contract, UploadedFileContract uploadedImage) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

			repository.HandleTransaction(ctx => {

				var tag = ctx.Load(contract.Name);

				permissionContext.VerifyEntryEdit(tag);

				var diff = new TagDiff();

				var newAliasedTo = contract.AliasedTo ?? string.Empty;
				if (!Tag.Equals(tag.AliasedTo, contract.AliasedTo)) {
					diff.AliasedTo = true;
					tag.AliasedTo = GetRealTag(ctx, newAliasedTo, tag);
				}

				if (tag.CategoryName != contract.CategoryName)
					diff.CategoryName = true;

				if (tag.Description != contract.Description)
					diff.Description = true;

				if (!Tag.Equals(tag.Parent, contract.Parent)) {

					var newParent = GetRealTag(ctx, contract.Parent, tag);

					if (!Equals(newParent, tag.Parent)) {
						diff.Parent = true;
						tag.SetParent(newParent);						
					}

				}

				if (tag.Status != contract.Status)
					diff.Status = true;

				tag.CategoryName = contract.CategoryName;
				tag.Description = contract.Description;
				tag.Status = contract.Status;

				if (uploadedImage != null) {

					diff.Picture = true;

					var thumb = new EntryThumb(tag, uploadedImage.Mime);
					tag.Thumb = thumb;
					var thumbGenerator = new ImageThumbGenerator(imagePersister);
					thumbGenerator.GenerateThumbsAndMoveImage(uploadedImage.Stream, thumb, ImageSizes.Original | ImageSizes.SmallThumb, originalSize: 500);

				}

				var logStr = string.Format("updated properties for tag {0} ({1})", entryLinkFactory.CreateEntryLink(tag), diff.ChangedFieldsString);
				ctx.AuditLogger.AuditLog(logStr);
				Archive(ctx, tag, diff, EntryEditEvent.Updated);

				ctx.Update(tag);

			});

		}


	}

}