using System;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Helpers;
using WebApi.OutputCache.V2;

namespace VocaDb.Web.Controllers.Api {
	
	/// <summary>
	/// API queries for tags.
	/// </summary>
	[RoutePrefix("api/tags")]
	public class TagApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly TagQueries queries;
		private readonly IEntryImagePersisterOld thumbPersister;

		public TagApiController(TagQueries queries, IEntryImagePersisterOld thumbPersister) {
			this.queries = queries;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Deletes a comment.
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </summary>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) {

			queries.HandleTransaction(ctx => queries.Comments(ctx).Delete(commentId));

		}

		/// <summary>
		/// Gets a tag by name.
		/// </summary>
		/// <param name="name">Tag name (required).</param>
		/// <param name="fields">
		/// List of optional fields (optional). 
		/// </param>
		/// <example>http://vocadb.net/api/tags/byName/vocarock</example>
		/// <returns>Tag data.</returns>
		[Route("byName/{name}")]
		public TagForApiContract GetByName(string name, TagOptionalFields fields = TagOptionalFields.None) {
			
			var tag = queries.GetTag(name, t => new TagForApiContract(t, thumbPersister, WebHelper.IsSSL(Request), fields));

			return tag;

		}

		/// <summary>
		/// Gets a list of tag category names.
		/// </summary>
		[Route("categoryNames")]
		public string[] GetCategoryNamesList(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto) {
		
			return queries.FindCategories(SearchTextQuery.Create(query, nameMatchMode));

		}

		/// <summary>
		/// Gets a list of comments for a tag.
		/// Note: pagination and sorting might be added later.
		/// </summary>
		/// <param name="tagId">ID of the tag whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{tagId:int}/comments")]
		public PartialFindResult<CommentForApiContract> GetComments(int tagId) {

			return new PartialFindResult<CommentForApiContract>(queries.GetComments(tagId), 0);

		}

		/// <summary>
		/// Find tags.
		/// </summary>
		/// <param name="query">Tag name query (optional).</param>
		/// <param name="allowAliases">Whether to allow tag alises. If this is false, alises will not be included.</param>
		/// <param name="categoryName">Filter tags by category (optional). If specified, this must be an exact match (case insensitive).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Exact).</param>
		/// <param name="sort">Sort rule (optional, by default tags are sorted by name).Possible values are Name and UsageCount.</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Description, MainPicture.
		/// </param>
		/// <returns>Page of tags.</returns>
		/// <example>http://vocadb.net/api/tags?query=voca&amp;nameMatchMode=StartsWith</example>
		[Route("")]
		public PartialFindResult<TagForApiContract> GetList(
			string query = "",
			bool allowAliases = false,
			string categoryName = "",
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			TagSortRule? sort = null,
			TagOptionalFields fields = TagOptionalFields.None) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var ssl = WebHelper.IsSSL(Request);
			var queryParams = new CommonSearchParams(TagSearchTextQuery.Create(query, nameMatchMode), false, false, false);
			var paging = new PagingProperties(start, maxResults, getTotalCount);

			var tags = queries.Find(t => new TagForApiContract(t, thumbPersister, ssl, fields), 
				queryParams, paging, 
				allowAliases, categoryName, sort ?? TagSortRule.Name);

			return tags;

		}

		/// <summary>
		/// Find tag names by a part of name.
		/// 
		/// Matching is done anywhere from the name.
		/// Spaces are automatically converted into underscores.
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
			int maxResults = 10) {
			
			return queries.FindNames(TagSearchTextQuery.Create(query), null, TagSortRule.Name, allowAliases, true, maxResults);

		}

		/// <summary>
		/// Gets the most common tags in a category.
		/// </summary>
		/// <param name="categoryName">Tag category, for example "Genres". Optional - if not specified, no filtering is done.</param>
		/// <returns>List of names of the most commonly used tags in that category.</returns>
		[Route("top")]
		[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
		public TagBaseContract[] GetTopTags(string categoryName = null) {

			return queries.Find(t => new TagBaseContract(t), new CommonSearchParams(), new PagingProperties(0, 15, false), false, string.Empty, TagSortRule.UsageCount)
				.Items.OrderBy(t => t.Name).ToArray();

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
		public void PostEditComment(int commentId, CommentForApiContract contract) {

			queries.HandleTransaction(ctx => queries.Comments(ctx).Update(commentId, contract));

		}

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="tagId">ID of the tag for which to create the comment.</param>
		/// <param name="contract">Comment data. Message and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{tagId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int tagId, CommentForApiContract contract) {

			return queries.CreateComment(tagId, contract);

		}

	}

}