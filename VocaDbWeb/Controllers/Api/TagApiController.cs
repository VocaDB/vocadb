using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using WebApi.OutputCache.V2;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for tags.
	/// </summary>
	[RoutePrefix("api/tags")]
	public class TagApiController : ApiController
	{
		private const int absoluteMax = 100;
		private const int defaultMax = 10;
		private readonly TagQueries queries;
		private readonly IAggregatedEntryImageUrlFactory thumbPersister;

		public TagApiController(TagQueries queries, IAggregatedEntryImageUrlFactory thumbPersister)
		{
			this.queries = queries;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Deletes a tag.
		/// </summary>
		/// <param name="id">ID of the tag to be deleted.</param>
		/// <param name="notes">Notes (optional).</param>
		/// <param name="hardDelete">
		/// If true, the entry is hard deleted. Hard deleted entries cannot be restored normally, but they will be moved to trash.
		/// If false, the entry is soft deleted, meaning it can still be restored.
		/// </param>
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "", bool hardDelete = false)
		{
			notes = notes ?? string.Empty;

			if (hardDelete)
			{
				queries.MoveToTrash(id, notes);
			}
			else
			{
				queries.Delete(id, notes);
			}
		}

		/// <summary>
		/// Deletes a comment.
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) => queries.DeleteComment(commentId);

		/// <summary>
		/// Gets a tag by ID.
		/// </summary>
		/// <param name="id">Tag ID (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). 
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/tags/1</example>
		/// <returns>Tag data.</returns>
		[Route("{id:int}")]
		public TagForApiContract GetById(int id, TagOptionalFields fields = TagOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default)
			=> queries.LoadTag(id, t => new TagForApiContract(t, thumbPersister, lang, fields));

		/// <summary>
		/// DEPRECATED. Gets a tag by name.
		/// </summary>
		/// <param name="name">Tag name (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). 
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/tags/byName/vocarock</example>
		/// <returns>Tag data.</returns>
		[Route("byName/{name}")]
		[Obsolete]
		public TagForApiContract GetByName(string name, TagOptionalFields fields = TagOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default)
			=> queries.GetTagByName(name, t => new TagForApiContract(t, thumbPersister, lang, fields));

		/// <summary>
		/// Gets a list of tag category names.
		/// </summary>
		[Route("categoryNames")]
		[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
		public string[] GetCategoryNamesList(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto) => queries.FindCategories(SearchTextQuery.Create(query, nameMatchMode));

		/// <summary>
		/// Gets a list of child tags for a tag.
		/// Only direct children will be included.
		/// </summary>
		/// <param name="tagId">ID of the tag whose children to load.</param>
		/// <param name="fields">List of optional fields (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>List of child tags.</returns>
		/// <example>http://vocadb.net/api/tags/481/children</example>
		[Route("{tagId:int}/children")]
		public TagForApiContract[] GetChildTags(int tagId,
			TagOptionalFields fields = TagOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => queries.GetChildTags(tagId, fields, lang);

		/// <summary>
		/// Gets a list of comments for a tag.
		/// Note: pagination and sorting might be added later.
		/// </summary>
		/// <param name="tagId">ID of the tag whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{tagId:int}/comments")]
		public PartialFindResult<CommentForApiContract> GetComments(int tagId) => new PartialFindResult<CommentForApiContract>(queries.GetComments(tagId), 0);

		/// <summary>
		/// Find tags.
		/// </summary>
		/// <param name="query">Tag name query (optional).</param>
		/// <param name="allowChildren">Whether to allow child tags. If this is false, only root tags (that aren't children of any other tag) will be included.</param>
		/// <param name="categoryName">Filter tags by category (optional). If specified, this must be an exact match (case insensitive).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Exact).</param>
		/// <param name="sort">Sort rule (optional, by default tags are sorted by name).Possible values are Name and UsageCount.</param>
		/// <param name="preferAccurateMatches">
		/// Whether the search should prefer accurate matches. 
		/// If this is true, entries that match by prefix will be moved first, instead of being sorted alphabetically.
		/// Requires a text query. Does not support pagination.
		/// This is mostly useful for autocomplete boxes.
		/// </param>
		/// <param name="fields">List of optional fields (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of tags.</returns>
		/// <example>http://vocadb.net/api/tags?query=voca&amp;nameMatchMode=StartsWith</example>
		[Route("")]
		public PartialFindResult<TagForApiContract> GetList(
			string query = "",
			bool allowChildren = true,
			string categoryName = "",
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			TagSortRule? sort = null,
			bool preferAccurateMatches = false,
			TagOptionalFields fields = TagOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			TagTargetTypes target = TagTargetTypes.All)
		{
			maxResults = Math.Min(maxResults, fields != TagOptionalFields.None ? absoluteMax : int.MaxValue);
			var queryParams = new TagQueryParams(new CommonSearchParams(TagSearchTextQuery.Create(query, nameMatchMode), false, preferAccurateMatches),
				new PagingProperties(start, maxResults, getTotalCount))
			{
				AllowChildren = allowChildren,
				CategoryName = categoryName,
				SortRule = sort ?? TagSortRule.Name,
				LanguagePreference = lang,
				Target = target
			};

			var tags = queries.Find(queryParams, fields, lang);

			return tags;
		}

		[Route("entry-type-mappings")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public TagEntryMappingContract[] GetEntryMappings() => queries.GetEntryMappings();

		[Route("mappings")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public PartialFindResult<TagMappingContract> GetMappings(
			int start = 0, int maxEntries = defaultMax, bool getTotalCount = false) => queries.GetMappings(new PagingProperties(start, maxEntries, getTotalCount));

		/// <summary>
		/// Find tag names by a part of name.
		/// 
		/// Matching is done anywhere from the name.
		/// </summary>
		/// <param name="query">Tag name query, for example "rock".</param>
		/// <param name="allowAliases">
		/// Whether to find tags that are aliases of other tags as well. 
		/// If false, only tags that are not aliases will be listed.
		/// </param>
		/// <param name="maxResults">Maximum number of search results.</param>
		/// <returns>
		/// List of tag names, for example "vocarock", matching the query. Cannot be null.
		/// </returns>
		[Route("names")]
		public string[] GetNames(
			string query = "", bool allowAliases = true,
			int maxResults = 10) => queries.FindNames(TagSearchTextQuery.Create(query), allowAliases, maxResults);

		/// <summary>
		/// Gets the most common tags in a category.
		/// </summary>
		/// <param name="categoryName">Tag category, for example "Genres". Optional - if not specified, no filtering is done.</param>
		/// <param name="entryType">Tag usage entry type. Optional - if not specified, all entry types are included.</param>
		/// <param name="maxResults">Maximum number of tags to return.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>List of names of the most commonly used tags in that category.</returns>
		[Route("top")]
		[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
		public TagBaseContract[] GetTopTags(string categoryName = null, EntryType? entryType = null,
			int maxResults = 15,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => queries.GetTopTags(categoryName, entryType, maxResults, lang);

		/// <summary>
		/// Creates a new report.
		/// </summary>
		/// <param name="tagId">Tag to be reported.</param>
		/// <param name="reportType">Report type.</param>
		/// <param name="notes">Notes. Optional.</param>
		/// <param name="versionNumber">Version to be reported. Optional.</param>
		[Route("{tagId:int}/reports")]
		[RestrictBannedIP]
		public void PostReport(int tagId, TagReportType reportType, string notes, int? versionNumber) => queries.CreateReport(tagId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);

		/// <summary>
		/// Creates a new tag.
		/// </summary>
		/// <param name="name">Tag English name. Tag names must be unique.</param>
		/// <returns>The created tag.</returns>
		/// <response code="200">OK</response>		
		/// <response code="400">If tag name is already in use</response>
		[Route("")]
		[Authorize]
		public async Task<TagBaseContract> PostNewTag(string name)
		{
			try
			{
				return await queries.Create(name);
			}
			catch (DuplicateTagNameException)
			{
				throw new HttpBadRequestException("Tag name is already in use");
			}
		}

		/// <summary>
		/// Updates a comment.
		/// Normal users can edit their own comments, moderators can edit all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be edited.</param>
		/// <param name="contract">New comment data. Only message can be edited.</param>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) => queries.PostEditComment(commentId, contract);

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="tagId">ID of the tag for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{tagId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int tagId, CommentForApiContract contract) => queries.CreateComment(tagId, contract);

		[Authorize]
		[Route("entry-type-mappings")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public void PutEntryMappings(IEnumerable<TagEntryMappingContract> mappings)
		{
			if (mappings == null)
			{
				throw new HttpBadRequestException("Mappings cannot be null");
			}

			queries.UpdateEntryMappings(mappings.ToArray());
		}

		[Authorize]
		[Route("mappings")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public void PutMappings(IEnumerable<TagMappingContract> mappings)
		{
			if (mappings == null)
			{
				throw new HttpBadRequestException("Mappings cannot be null");
			}

			queries.UpdateMappings(mappings.ToArray());
		}
	}
}